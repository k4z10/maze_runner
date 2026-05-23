using System.Text;
using maze_runner.Model.Core;
using maze_runner.Model.Dungeon.Map;
using maze_runner.Model.Entities;
using maze_runner.Model.Entities.Player;
using maze_runner.Model.Items.Models;
using maze_runner.Network.DTOs.GameState;

namespace maze_runner.Server;

public static class SnapshotGenerator
{
    public static GameStateSnapshotDto GenerateSnapshot(ILevelContext ctx)
    {
        var map = ctx.Map;
        var entityManager = ctx.EntityManager;
        var snapshot = new GameStateSnapshotDto
        {
            Entities = [],
            Map = CreateMapDto(map),
            LevelMeta = CreateLevelMetaDto(ctx)
        };

        foreach (var entity in entityManager.Entities)
            snapshot.Entities.Add(CreateEntityDto(entity));
        
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
            },
            Inventory = entity.Inventory == null ? null : new InventoryDto()
            {
                LeftHand = entity.Inventory.LeftHand != null ? CreateItemDto(entity.Inventory.LeftHand) : null,
                RightHand = entity.Inventory.RightHand != null ? CreateItemDto(entity.Inventory.RightHand) : null,
                Items = entity.Inventory.Items.Select(CreateItemDto).ToList(),
                Coins =  entity.Inventory.Coins,
                Gold = entity.Inventory.Gold,
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