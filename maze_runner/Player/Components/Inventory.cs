using System.Collections.ObjectModel;

namespace maze_runner.Player.Components;
using Items.Models;

public class Inventory
{
    public ObservableCollection<Item> Items = new();
    public int CurrentIndex = 0;
    
    public int Gold { get; set; }
    public int Coins { get; set; }    
    
    public Item? LeftHand = null;
    public Item? RightHand = null;
}