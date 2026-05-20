using System.Text;
using maze_runner.Core;
using maze_runner.Model.Dungeon.Map;
using maze_runner.Model.Entities.Player;
using maze_runner.Model.Items.Models;
using maze_runner.Network.DTOs.GameState;

namespace maze_runner.ServerEngine;
using Model.Entities;

public static class SnapshotGenerator
{
    public static GameStateSnapshotDto GenerateSnapshot(ILevelContext ctx)
    {
        var map = ctx.Map;
        var entityManager = ctx.EntityManager;
        var snapshot = new GameStateSnapshotDto
        {
            Entities = [],
            Players = [],
            Map = CreateMapDto(map),
            LevelMeta = CreateLevelMetaDto(ctx)
        };

        foreach (var entity in entityManager.Mobs)
            snapshot.Entities.Add(CreateEntityDto(entity));
        
        foreach (var player in entityManager.Players)
            snapshot.Players.Add(CreatePlayerDto(player));
        
        return snapshot;
    }

    private static EntityDto CreateEntityDto(Entity entity)
    {
        return new EntityDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Symbol = entity.Symbol,
            Row = entity.Position.Row,
            Col = entity.Position.Col,
            Health = entity.Health,
            MaxHealth = entity.MaxHealth,
            IsAlive = entity.IsAlive,
            Damage = entity.EffectiveDamage,
            Defense = entity.EffectiveDefense,
            Stats = new AttributesDto
            {
                Strength = entity.CurrentStats.Strength,
                Dexterity = entity.CurrentStats.Dexterity,
                Resistance = entity.CurrentStats.Resistance,
                Stamina = entity.CurrentStats.Stamina,
                Luck = entity.CurrentStats.Luck,
                Wisdom = entity.CurrentStats.Wisdom
            }
        };
    }

    private static ItemDto CreateItemDto(Item item)
    {
        AttributesDto? modifiersDto = null;
        var equippable = item.GetEquippableFeature();

        if (equippable != null)
        {
            var deltaStats = new Attributes();
            equippable.ApplyStatModifiers(ref deltaStats);

            if (HasAnyModifiers(deltaStats))
            {
                modifiersDto = new AttributesDto
                {
                    Strength = deltaStats.Strength,
                    Dexterity = deltaStats.Dexterity,
                    Resistance = deltaStats.Resistance,
                    Stamina = deltaStats.Stamina,
                    Luck = deltaStats.Luck,
                    Wisdom = deltaStats.Wisdom
                };
            }
        }

        var weapon = item.GetWeaponFeature();
        var currency = item.GetCurrencyFeature();

        return new ItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Symbol = item.TileSymbol,
            Damage = weapon?.Damage,
            Amount = currency?.Amount,
            StatModifiers = modifiersDto
        };
    }

    private static bool HasAnyModifiers(Attributes mods) => mods.Strength != 0 || mods.Dexterity != 0 || mods.Resistance != 0 || mods.Stamina != 0 || mods.Luck != 0 || mods.Wisdom != 0;

    private static PlayerDto CreatePlayerDto(Player player)
    {
        return new PlayerDto
        {
            Id = player.Id,
            Name = "Player",
            Symbol = player.Symbol, 
            Row = player.Position.Row,
            Col = player.Position.Col,
            Health = player.Health,
            MaxHealth = player.MaxHealth,
        
            Stats = new AttributesDto
            {
                Strength = player.CurrentStats.Strength,
                Dexterity = player.CurrentStats.Dexterity,
                Resistance = player.CurrentStats.Resistance,
                Stamina = player.CurrentStats.Stamina,
                Luck = player.CurrentStats.Luck,
                Wisdom = player.CurrentStats.Wisdom
            },

            Inventory = new InventoryDto
            {
                LeftHand = player.Inventory.LeftHand != null ? CreateItemDto(player.Inventory.LeftHand) : null,
                RightHand = player.Inventory.RightHand != null ? CreateItemDto(player.Inventory.RightHand) : null,
                Items = player.Inventory.Items.Select(CreateItemDto).ToList(),
                Coins =  player.Inventory.Coins,
                Gold = player.Inventory.Gold,
            }
        };
    }

    private static MapDto CreateMapDto(Map map)
    {
        var topologyBuilder = new StringBuilder(map.Cols * map.Rows);
        var droppedItems = new List<DroppedItemDto>();

        for (int r = 0; r < map.Rows; r++)
        {
            for (int c = 0; c < map.Cols; c++)
            {
                var tile = map.GetTile(r, c);
            
                topologyBuilder.Append(tile.GetTileSymbol());

                if (tile.Items.Count <= 0) continue;
                foreach (var item in tile.Items)
                {
                    droppedItems.Add(new DroppedItemDto
                    {
                        Row = r,
                        Col = c,
                        Item = CreateItemDto(item)
                    });
                }
            }
        }

        return new MapDto
        {
            Width = map.Cols,
            Height = map.Rows,
            Topology = topologyBuilder.ToString(),
            DroppedItems = droppedItems
        };
    }

    private static LevelMetaDto CreateLevelMetaDto(ILevelContext ctx)
    {
        var meta = new LevelMetaDto
        {
            Commands = [],
            Description = ctx.Description,
            Name = ctx.LevelName
        };

        foreach (var kvp in ctx.CommandRegistry.KeyBindings)
        {
            meta.Commands.Add(new CommandBindingDto
            {
                CommandId = kvp.Value,
                Description = ctx.CommandRegistry.Descriptions[kvp.Value],
                Key = kvp.Key
            });
        }
        
        return meta;
    }
    
}