/*
 * Copyright 2017 Maxst, Inc. All Rights Reserved.
 */

package com.maxst.ar.android_extension;

import android.app.Activity;
import android.util.DisplayMetrics;
import android.util.Log;
import android.view.SurfaceView;
import android.view.View;
import android.view.ViewGroup;

public class AndroidExtension {

	private static String TAG = AndroidExtension.class.getSimpleName();

	private static AndroidExtension instance = null;

	public static AndroidExtension init(Activity activity) {
		if (instance == null) {
			instance = new AndroidExtension(activity);
		}

		return instance;
	}

	public static void deinit() {
		instance = null;
	}

	private Activity activity;
	private ViewGroup surfaceViewParent = null;
	private SurfaceView unitySurfaceView = null;

	private AndroidExtension(Activity activity) {
		this.activity = activity;
		surfaceViewParent = (ViewGroup)getSurfaceViewInViewTree((ViewGroup)activity.getWindow().getDecorView()).getParent();
	}

	public void resizeSurface(final boolean toggle) {
		Log.d(TAG, "Resize surface as : " + toggle);

		activity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				if (surfaceViewParent != null) {
					DisplayMetrics metrics = activity.getResources().getDisplayMetrics();

					Log.d(TAG, "Screen width : " + metrics.widthPixels);
					Log.d(TAG, "Screen height : " + metrics.heightPixels);

					if (unitySurfaceView != null) {
						Log.d(TAG, "SurfaceView is not null");
						ViewGroup.LayoutParams layoutParams = unitySurfaceView.getLayoutParams();
						if (toggle) {
							layoutParams.width = metrics.widthPixels / 2;
							layoutParams.height = metrics.heightPixels / 2;
//							if (surfaceViewParent instanceof FrameLayout) {
//								FrameLayout.LayoutParams fl = (FrameLayout.LayoutParams) layoutParams;
//								fl.leftMargin = metrics.widthPixels / 4;
//								fl.topMargin = metrics.heightPixels / 4;
//							}
							unitySurfaceView.setLayoutParams(layoutParams);
						} else {
							layoutParams.width = metrics.widthPixels;
							layoutParams.height = metrics.heightPixels;
//							if (surfaceViewParent instanceof FrameLayout) {
//								FrameLayout.LayoutParams fl = (FrameLayout.LayoutParams) layoutParams;
//								fl.leftMargin = 0;
//								fl.topMargin = 0;
//							}
							unitySurfaceView.setLayoutParams(layoutParams);
						}
					}
				}
			}
		});
	}

	private SurfaceView getSurfaceViewInViewTree(ViewGroup viewGroup) {

		int numOfChild = viewGroup.getChildCount();
		for (int i = 0; i < numOfChild; i++) {
			View view = viewGroup.getChildAt(i);
			if (view instanceof SurfaceView) {
				return unitySurfaceView = (SurfaceView) view;
			} else if (view instanceof ViewGroup) {
				SurfaceView surfaceView = getSurfaceViewInViewTree((ViewGroup) view);
				if (surfaceView != null) {
					return surfaceView;
				}
			}
		}

		return null;
	}
}