namespace PsychedelicExperience.Business

    /// This base class has id and a list of events related to current domain object.
    [<AbstractClass>]
    type AggregateRoot<'event>(id) =
        let mutable changes = []
        member x.Id = id
        member x.GetUncommittedChanges = changes
        member x.MarkChangesAsCommitted = changes <- []
        abstract member Apply : 'event -> unit

        member x.ApplyChange (evt:'event) = 
            x.Apply evt
            changes <- evt :: changes
