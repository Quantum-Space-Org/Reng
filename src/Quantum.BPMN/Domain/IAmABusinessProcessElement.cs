namespace Reng.BPMN.Domain.Domain;

public abstract record IAmABusinessProcessElement(string Id, string Name)
{
    public string Token { get; private set; }

    public BusinessProcessElementStatus Status { get; protected set; }
        = BusinessProcessElementStatus.NotStarted;

    public ICollection<IAmABusinessProcessElement> OutgoingList { get; } = new List<IAmABusinessProcessElement>();
    public ICollection<IAmABusinessProcessElement> IncomingList { get; } = new List<IAmABusinessProcessElement>();
    public bool HasCompensate { get; private set; }

    public ICollection<IAmABusinessProcessElement> GetOutgoings() => OutgoingList;

    public virtual void SetOutgoing(IAmABusinessProcessElement outgoing)
        => OutgoingList.Add(outgoing);

    public virtual void SetIncoming(IAmABusinessProcessElement outgoing)
        => IncomingList.Add(outgoing);

    public abstract IAmABusinessProcessElement GetNext();

    public void GoToInProgressState() => Status = BusinessProcessElementStatus.InProgress;
    public void GoToCompletedSuccessfullyState() => Status = BusinessProcessElementStatus.CompletedSuccessfully;
    public void GoToFailedState() => Status = BusinessProcessElementStatus.Failed;

    public void SetToken(string token)
        => Token = token;
    public bool HasToken() => !string.IsNullOrWhiteSpace(Token);
    public void SetHasCompensate(bool value)
        => this.HasCompensate = value;
}

public enum BusinessProcessElementStatus
{
    NotStarted,
    InProgress,
    Waiting,
    Suspended,
    CompletedSuccessfully,
    Failed,
}