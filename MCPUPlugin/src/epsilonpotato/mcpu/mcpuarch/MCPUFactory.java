package epsilonpotato.mcpu.mcpuarch;

import org.bukkit.World;
import org.bukkit.entity.Player;

import epsilonpotato.mcpu.core.MCPUCore;
import epsilonpotato.mcpu.core.SquareEmulatedProcessorFactory;

public final class MCPUFactory extends SquareEmulatedProcessorFactory<MCPUProcessor>
{
    @Override
    public MCPUProcessor createProcessor(MCPUCore caller, Player p, World w, int x, int y, int z, int iosidecount)
    {
        try
        {
            return new MCPUProcessor(p, w, x, y, z, iosidecount);
        }
        catch (Exception e)
        {
            MCPUCore.Error(p, "Unable to create the processor due to the following reason:\n" + e);

            return null;
        }
    }
}
