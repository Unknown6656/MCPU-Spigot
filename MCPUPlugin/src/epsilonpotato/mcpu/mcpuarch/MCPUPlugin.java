package epsilonpotato.mcpu.mcpuarch;


import org.bukkit.command.Command;
import org.bukkit.command.CommandSender;
import org.bukkit.event.Listener;
import org.bukkit.event.world.WorldInitEvent;
import org.bukkit.event.world.WorldLoadEvent;
import org.bukkit.event.world.WorldSaveEvent;
import epsilonpotato.mcpu.core.MCPUCore;


public final class MCPUPlugin extends MCPUCore implements Listener
{
    @Override
    public void onWorldSaveEvent(WorldSaveEvent event)
    {
        // TODO Auto-generated method stub
    }
    
    @Override
    public void onWorldLoadEvent(WorldLoadEvent event)
    {
        // TODO Auto-generated method stub
    }
    
    @Override
    public void onWorldInitEvent(WorldInitEvent event)
    {
        // TODO Auto-generated method stub
    }
    
    @Override
    public boolean onUnprocessedCommand(CommandSender sender, Command command, String label, String[] args)
    {
        // TODO Auto-generated method stub
        return false;
    }
}
