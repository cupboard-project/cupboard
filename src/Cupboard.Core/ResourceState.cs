namespace Cupboard;

public enum ResourceState
{
    Unknown = 0,
    Changed,
    Unchanged,
    Executed,
    Skipped,
    ManuallySkipped,
    Error,
}