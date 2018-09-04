namespace maxstAR
{
    [System.SerializableAttribute]
    class CloudRecognitionData
    {
        public string ImgId { get; set; }
        public string Custom { get; set; }
        public string Track2dMapUrl { get; set; }
        public string Name { get; set; }
        public string ImgGSUrl { get; set; }
        public float RealWidth { get; set; }
    }
}
