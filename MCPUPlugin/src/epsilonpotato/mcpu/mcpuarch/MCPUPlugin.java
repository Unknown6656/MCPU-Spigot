package epsilonpotato.mcpu.mcpuarch;

import java.util.HashMap;

import org.bukkit.command.Command;
import org.bukkit.command.CommandSender;
import org.bukkit.event.Listener;

import epsilonpotato.mcpu.core.SquareEmulatedProcessorFactory;
import epsilonpotato.mcpu.util.Tuple;
import epsilonpotato.mcpu.core.MCPUCore;


public final class MCPUPlugin extends MCPUCore implements Listener
{
    private static final HashMap<String, Tuple<String, String>> usageoptions = new HashMap<>();
    
    static
    {
       //TODO : possibly add more options to `usageoptions`
    }
    
    public MCPUPlugin()
    {
        super(usageoptions);
        
        System.out.println("Loaded " + MCPUOpcode.OpcodesS.size() + " MCPU Instructions:\n\t " + String.join(", ", MCPUOpcode.OpcodesS.keySet()));
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
            error(null, "Unable to register component factories:\n" + e.toString());
        }
    }
    
    @Override
    public String getAboutText()
    {
        return "        Copyright (c) 2017 Unknown6656\n" +
               "        Version v." + getDescription().getVersion();
    }
}
