using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DagligaHatet {

    public abstract class Skill {

        protected Texture2D selectedTile;

        public Skill(Texture2D selectedTile) {
            this.selectedTile = selectedTile;
        }

        public abstract void PrepareSkill(List<PlayerCharacter> playerCharacters, int indexNumber, List<Tile> map, List<Tile> selectedTiles);

        public abstract void InvokeSkill(List<PlayerCharacter> playerCharacters, int indexNumber, List<Tile> map, List<Tile> selectedTiles, int tileNumber, SpriteBatch spriteBatch);

        public virtual void Draw(List<Tile> selectedTiles, SpriteBatch spriteBatch) {
            selectedTiles.ForEach(x => x.Draw(spriteBatch, selectedTile));
        }
    }

    #region Attacks 

    public class Attack : Skill {
        protected Texture2D cross;


        /// <param name="selectedTile">Sword texture</param>
        /// <param name="cross">Cross texture</param>
        public Attack(Texture2D selectedTile, Texture2D cross) : base(selectedTile) {
            this.cross = cross;
        }

        public override void PrepareSkill(List<PlayerCharacter> playerCharacters, int indexNumber, List<Tile> map, List<Tile> selectedTiles) {
            selectedTiles.Where(x => !x.Occupied).ToList().ForEach(x => AnimationEngine.AddPermanent("selectedTiles", cross, x.Position));
            selectedTiles.Where(x => x.Occupied).ToList().ForEach(x => AnimationEngine.AddPermanent("selectedTiles", selectedTile, x.Position));
        }
    
        public override void InvokeSkill(List<PlayerCharacter> playerCharacters, int indexNumber, List<Tile> map, List<Tile> selectedTiles, int tileNumber, SpriteBatch spriteBatch) {
            Console.WriteLine(playerCharacters.Find(x => x.MapPosition == selectedTiles[tileNumber].MapPosition).Health);
            playerCharacters.Where(x => x.MapPosition == selectedTiles[tileNumber].MapPosition).ToList().ForEach(x => x.Health = x.Health - playerCharacters[indexNumber].Damage);

            Console.WriteLine(playerCharacters.Find(x => x.MapPosition == selectedTiles[tileNumber].MapPosition).Health);
            AnimationEngine.ClearPermanent("selectedTiles");
            selectedTiles.Clear();
        }
    }

    public class AttackMelee : Attack {
        public AttackMelee(Texture2D selectedTile, Texture2D cross) : base(selectedTile, cross) {

        }

        public override void PrepareSkill(List<PlayerCharacter> playerCharacters, int indexNumber, List<Tile> map, List<Tile> selectedTiles) {
            selectedTiles.AddRange(map.Where(x => Math.Abs(x.MapPosition.X - playerCharacters[indexNumber].MapPosition.X) + Math.Abs(x.MapPosition.Y - playerCharacters[indexNumber].MapPosition.Y) < playerCharacters[indexNumber].Range));

            selectedTiles.RemoveAll(x => x.MapPosition == playerCharacters[indexNumber].MapPosition);
            base.PrepareSkill(playerCharacters, indexNumber, map, selectedTiles);
        }
    }

    public class AttackRangeCross : Attack {
        public AttackRangeCross(Texture2D selectedTile, Texture2D cross) : base(selectedTile, cross) {

        }

        public override void PrepareSkill(List<PlayerCharacter> playerCharacters, int indexNumber, List<Tile> map, List<Tile> selectedTiles) {
            selectedTiles.AddRange(map.Where(x => Math.Abs(x.MapPosition.X - playerCharacters[indexNumber].MapPosition.X) < playerCharacters[indexNumber].Range &&
                                x.MapPosition.Y == playerCharacters[indexNumber].MapPosition.Y ||
                                Math.Abs(x.MapPosition.Y - playerCharacters[indexNumber].MapPosition.Y) < playerCharacters[indexNumber].Range &&
                                x.MapPosition.X == playerCharacters[indexNumber].MapPosition.X));

            selectedTiles.RemoveAll(x => x.MapPosition == playerCharacters[indexNumber].MapPosition);
            base.PrepareSkill(playerCharacters, indexNumber, map, selectedTiles);
        }
    }

    public class AttackRangeXCross : Attack {
        public AttackRangeXCross(Texture2D selectedTile, Texture2D cross) : base(selectedTile, cross) {

        }

        public override void PrepareSkill(List<PlayerCharacter> playerCharacters, int indexNumber, List<Tile> map, List<Tile> selectedTiles) {
            selectedTiles.AddRange(map.Where(x => Math.Abs(x.MapPosition.X - playerCharacters[indexNumber].MapPosition.X) < playerCharacters[indexNumber].Range &&
                                x.MapPosition.Y == playerCharacters[indexNumber].MapPosition.Y ||
                                Math.Abs(x.MapPosition.Y - playerCharacters[indexNumber].MapPosition.Y) < playerCharacters[indexNumber].Range &&
                                x.MapPosition.X == playerCharacters[indexNumber].MapPosition.X));
            selectedTiles.AddRange(map.Where(x => Math.Abs(x.MapPosition.X - playerCharacters[indexNumber].MapPosition.X) == Math.Abs(x.MapPosition.Y - playerCharacters[indexNumber].MapPosition.Y) && Math.Abs(x.MapPosition.X - playerCharacters[indexNumber].MapPosition.X) + Math.Abs(x.MapPosition.Y - playerCharacters[indexNumber].MapPosition.Y) < playerCharacters[indexNumber].Range * 1.2));

            selectedTiles.RemoveAll(x => x.MapPosition == playerCharacters[indexNumber].MapPosition);
            base.PrepareSkill(playerCharacters, indexNumber, map, selectedTiles);
        }
    }
    #endregion

    #region WizardSkills

    public class SkillWizardHeal : Skill {

        public SkillWizardHeal(Texture2D selectedTile) : base(selectedTile) {

        }

        public override void PrepareSkill(List<PlayerCharacter> playerCharacters, int indexNumber, List<Tile> map, List<Tile> selectedTiles) {

            selectedTiles.AddRange(map.Where(x => x.Occupied == true &&
            Math.Abs(x.MapPosition.X - playerCharacters[indexNumber].MapPosition.X) +
            Math.Abs(x.MapPosition.Y - playerCharacters[indexNumber].MapPosition.Y) < playerCharacters[indexNumber].SkillRange));
            selectedTiles.ForEach(x => AnimationEngine.AddPermanent("selectedTiles", selectedTile, x.Position, Vector2.Zero, 0.3f, 2));
        }

        public override void InvokeSkill(List<PlayerCharacter> playerCharacters, int indexNumber, List<Tile> map, List<Tile> selectedTiles, int tileNumber, SpriteBatch spriteBatch) {
            Console.WriteLine(playerCharacters.Find(x => x.MapPosition == selectedTiles[tileNumber].MapPosition).Health);
            playerCharacters.Where(x => x.MapPosition == selectedTiles[tileNumber].MapPosition).ToList().ForEach(x => x.Health = x.Health + playerCharacters[indexNumber].SkillDamage);

            Console.WriteLine(playerCharacters.Find(x => x.MapPosition == selectedTiles[tileNumber].MapPosition).Health);
            AnimationEngine.ClearPermanent("selectedTiles");
            selectedTiles.Clear();
        }
    }

    #endregion

    #region RangerSkills

    public class SkillRangerBomb : Skill {
        Texture2D Target { get; }
        public SkillRangerBomb(Texture2D selectedTile, Texture2D target) : base(selectedTile) {
            Target = target;
        }

        public override void PrepareSkill(List<PlayerCharacter> playerCharacters, int indexNumber, List<Tile> map, List<Tile> selectedTiles) {
            selectedTiles.AddRange(map.Where(x => (x.MapPosition.Y == playerCharacters[indexNumber].MapPosition.Y ||
                                x.MapPosition.X == playerCharacters[indexNumber].MapPosition.X) &&
                                (Math.Abs(x.MapPosition.X - playerCharacters[indexNumber].MapPosition.X) +
                                Math.Abs(x.MapPosition.Y - playerCharacters[indexNumber].MapPosition.Y) == playerCharacters[indexNumber].Range)));
            
            selectedTiles.ForEach(x => {
                map.Where(y => Math.Abs(y.MapPosition.X - x.MapPosition.X) + Math.Abs(y.MapPosition.Y - x.MapPosition.Y) < 2).ToList().Where(y => y.MapPosition != x.MapPosition).ToList().ForEach(y => {
                    AnimationEngine.AddPermanent("selectedTiles", selectedTile, new Vector2(y.Position.X, y.Position.Y));
                });
                AnimationEngine.AddPermanent("selectedTiles", Target, x.Position);
            });
        }

        public override void InvokeSkill(List<PlayerCharacter> playerCharacters, int indexNumber, List<Tile> map, List<Tile> selectedTiles, int tileNumber, SpriteBatch spriteBatch) {
            List<Tile> tempList = new List<Tile>();
            tempList.AddRange(map.Where(x => Math.Abs(x.MapPosition.X - selectedTiles[tileNumber].MapPosition.X) + Math.Abs(x.MapPosition.Y - selectedTiles[tileNumber].MapPosition.Y) < 2));

            foreach (var player in playerCharacters) {
                if (tempList.Exists(x => x.MapPosition == player.MapPosition)) {
                    Console.WriteLine(player.Health);
                    player.Health -= playerCharacters[indexNumber].SkillDamage;
                    Console.WriteLine(player.Health);
                }
            }

            selectedTiles.Clear();
        }
    }

    #endregion

    #region KnightSkills

    public class SkillKnightWhirlwind : Skill {
        Texture2D whirl;
        public SkillKnightWhirlwind(Texture2D selectedTile, Texture2D Whirlwind) : base(selectedTile) { whirl = Whirlwind; }

        public override void PrepareSkill(List<PlayerCharacter> playerCharacters, int indexNumber, List<Tile> map, List<Tile> selectedTiles) {
            selectedTiles.AddRange(map.Where(x => Math.Abs(x.MapPosition.X - playerCharacters[indexNumber].MapPosition.X) + Math.Abs(x.MapPosition.Y - playerCharacters[indexNumber].MapPosition.Y) < playerCharacters[indexNumber].SkillRange));

            selectedTiles.RemoveAll(x => x.MapPosition == playerCharacters[indexNumber].MapPosition);
            selectedTiles.ForEach(x => AnimationEngine.AddPermanent("selectedTiles", selectedTile, x.Position));

        }

        public override void InvokeSkill(List<PlayerCharacter> playerCharacters, int indexNumber, List<Tile> map, List<Tile> selectedTiles, int tileNumber, SpriteBatch spriteBatch) {
            AnimationEngine.AddQueued("Whirl", whirl, playerCharacters[indexNumber].Position, new Vector2(80, 80), 0.02f, 8, 0.32f, false);
            foreach (var tile in selectedTiles) {
                if (playerCharacters.Exists(x => x.MapPosition == tile.MapPosition)) {
                    Console.WriteLine(playerCharacters.Find(x => x.MapPosition == tile.MapPosition).Health);
                    playerCharacters.Where(x => x.MapPosition == tile.MapPosition).ToList().ForEach(x => x.Health = x.Health - playerCharacters[indexNumber].SkillDamage);
                    Console.WriteLine(playerCharacters.Find(x => x.MapPosition == tile.MapPosition).Health);
                }
            }
            AnimationEngine.AddPause(0.2f);
            AnimationEngine.AddReverseQueued("Whirl", whirl, playerCharacters[indexNumber].Position, new Vector2(80, 80), 0.02f, 8, 0.32f, false);
            foreach (var tile in selectedTiles) {
                if (playerCharacters.Exists(x => x.MapPosition == tile.MapPosition)) {
                    Console.WriteLine(playerCharacters.Find(x => x.MapPosition == tile.MapPosition).Health);
                    playerCharacters.Where(x => x.MapPosition == tile.MapPosition).ToList().ForEach(x => x.Health = x.Health - playerCharacters[indexNumber].SkillDamage);
                    Console.WriteLine(playerCharacters.Find(x => x.MapPosition == tile.MapPosition).Health);
                }
            }
            AnimationEngine.ClearPermanent("selectedTiles");
            selectedTiles.Clear();
        }

        public override void Draw(List<Tile> selectedTiles, SpriteBatch spriteBatch) {
            base.Draw(selectedTiles, spriteBatch);
        }
    }

    #endregion
}
