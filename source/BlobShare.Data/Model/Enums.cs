namespace Microsoft.Samples.DPE.BlobShare.Data.Model
{
    using System;

    public enum Privilege
    {
        Read = 1,
        Write = 2 
    }

    public enum EventType
    {
        None = 0,
        View = 1,
        Download = 2
    }

    public enum UserEventType
    {
        None = 0,
        Create = 1,
        Login = 2,
        Deactivation = 3,
        Activation = 4,
        UserHome = 5
    }
}