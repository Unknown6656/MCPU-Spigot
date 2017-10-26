package epsilonpotato.mcpu.mcpuarch;



import java.util.HashMap;

import org.bukkit.ChatColor;
import org.bukkit.command.Command;
import org.bukkit.command.CommandSender;
import org.bukkit.event.Listener;
import org.bukkit.event.world.WorldInitEvent;
import org.bukkit.event.world.WorldLoadEvent;
import org.bukkit.event.world.WorldSaveEvent;

import epsilonpotato.mcpu.core.SquareEmulatedProcessorFactory;
import epsilonpotato.mcpu.util.Tuple;
import epsilonpotato.mcpu.core.MCPUCore;


public final class MCPUPlugin extends MCPUCore implements Listener
{
    private static final HashMap<String, Tuple<String, String>> usageoptions = new HashMap<>();

    
    public MCPUPlugin()
    {
        // TODO : possibly add more options
        
        super(usageoptions);
    }
    

    @Override
    public void onWorldSaveEvent(WorldSaveEvent event)
    {
        // TODO Auto-generated method stub

        Print(ChatColor.AQUA, "W SAVE");
    }
    
    @Override
    public void onWorldLoadEvent(WorldLoadEvent event)
    {
        // TODO Auto-generated method stub

        Print(ChatColor.AQUA, "W LOAD");
    }
    
    @Override
    public void onWorldInitEvent(WorldInitEvent event)
    {
        // TODO Auto-generated method stub
        
        Print(ChatColor.AQUA, "W INIT");
    }
    
    @Override
    public boolean onUnprocessedCommand(CommandSender sender, Command command, String label, String[] args)
    {
        // TODO Auto-generated method stub
        return false;
    }

    @Override
    public void registerIntegratedCircuits()
    {
        try
        {
            SquareEmulatedProcessorFactory.registerFactory("mcpuarch", new MCPUFactory());
        }
        catch (Exception e)
        {
            Error(null, "Unable to register component factories:\n" + e.toString());
        }
    }
}
