namespace MipsEmu;

public struct Interval {
    public int start;
    public int length;

    public Interval(int start, int length) {
        this.start = start;
        this.length = length;
    }
}