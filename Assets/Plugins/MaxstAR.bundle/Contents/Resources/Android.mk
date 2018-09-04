LOCAL_PATH := $(call my-dir)

$(info LOCAL_PATH $(LOCAL_PATH))

include $(CLEAR_VARS)
include $(LOCAL_PATH)/../../../3rdparty/ZxingCpp/lib/Android/Android_prebuilt.mk

include $(CLEAR_VARS)
CODE_TRACKER_PATH := Tracker/CodeTracker

CODE_TRACKER_SRC_FILES := \
	$(CODE_TRACKER_PATH)/CodeTracker.cpp \
	$(CODE_TRACKER_PATH)/CodeTrackerAPI.cpp \
	$(CODE_TRACKER_PATH)/ImageReaderSource.cpp

CODE_TRACKER_INCLUDE := $(MAXSTAR_LOCAL_PATH)/../3rdparty/ZxingCpp/core/src

$(info CODE_TRACKER_INCLUDE $(CODE_TRACKER_INCLUDE))