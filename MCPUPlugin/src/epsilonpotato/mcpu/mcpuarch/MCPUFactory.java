package epsilonpotato.mcpu.mcpuarch;

import org.bukkit.Location;
import org.bukkit.entity.Player;

import epsilonpotato.mcpu.core.EmulatedProcessorFactory;
import epsilonpotato.mcpu.core.Triplet;

public final class MCPUFactory extends EmulatedProcessorFactory<MCPUProcessor>
{
    @Override
    public MCPUProcessor createProcessor(Player p, Location l, Triplet<Integer, Integer, Integer> size, int iocount)
    {
        return new MCPUProcessor(p, l.getWorld(), l.getBlockX(), l.getBlockY(), l.getBlockZ(), size.x, size.y, size.z, iocount);
    }
}
