// Every class in this collection swaps process wide state: InlineEngine.IsBuildServer, and for
// InlineRetireTests also DiffRunner.Disabled and the viewer port. Sharing a collection only keeps
// them apart from each other. Other collections would still run beside them, see the swapped state,
// and send their pending moves to a listener that is not theirs, so the collection runs alone
[CollectionDefinition("Inline", DisableParallelization = true)]
public class InlineCollection;
