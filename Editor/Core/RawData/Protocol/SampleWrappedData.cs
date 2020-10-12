using System.Collections;
using System.Collections.Generic;

namespace UTJ.ProfilerReader.RawData.Protocol
{
    // original struct for Sample
    public struct SampleWrappedData
    {
        public enum EType : int
        {
            None,
            Sample,
            SampleWithMetadata,
            SampleWithInstanceID,
        };
        public EType sampleType;
        public Sample sample;

        // instanceId
        public int instanceID;
        // metadata
        public byte metadataCount;
        public List<MetadataValue> metadatas;
    }
}