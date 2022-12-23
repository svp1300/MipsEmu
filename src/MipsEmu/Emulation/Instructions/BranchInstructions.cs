namespace MipsEmu.Emulation.Instructions;

public class BranchOnEqualInstruction :  BranchingInstruction {

    public override bool ShouldBranch(long rsValue, long rtValue) {
        return rsValue == rtValue;
    }
    
}

public class BranchOnNotEqualInstruction :  BranchingInstruction {

    public override bool ShouldBranch(long rsValue, long rtValue) {
        return rsValue != rtValue;
    }
    
}