package epsilonpotato.mcpu.mcpuarch;

import org.bukkit.Location;


final class MCPUBuildRegion
{
    private final int x, y, z, i;
    
    
    public MCPUBuildRegion(int x, int y, int z, int i)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.i = i;
    }
    
    public boolean Contains(Location l)
    {
        int bx = l.getBlockX();
        int by = l.getBlockY();
        int bz = l.getBlockZ();
        
        return (bx >= x - i) && (bx <= x + i) &&
               (by >= y - 1) && (by <= y + 2) &&
               (bz >= z - i) && (bz <= z + i);
    }
}
