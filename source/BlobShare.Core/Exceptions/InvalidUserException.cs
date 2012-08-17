namespace Microsoft.Samples.DPE.BlobShare.Core.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class InvalidUserException : Exception
    {
        private string description;

        public InvalidUserException(string message, string description)
            : base(message)
        {
            this.description = description;
        }

        public override string ToString()
        {
            return this.description;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
