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

        public abstract void InvokeSkill(List<PlayerCharacter> playerCharacters, int indexNumber, List<Tile> map, List<Tile> selectedTiles, int tileNumber);

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

            throw new NotImplementedException();
        }

        public override void InvokeSkill(List<PlayerCharacter> playerCharacters, int indexNumber, List<Tile> map, List<Tile> selectedTiles, int tileNumber) {
            Console.WriteLine(playerCharacters.Find(x => x.MapPosition == selectedTiles[tileNumber].MapPosition).Health);
            playerCharacters.Where(x => x.MapPosition == selectedTiles[tileNumber].MapPosition).ToList().ForEach(x => x.Health = x.Health - playerCharacters[indexNumber].Damage);

            Console.WriteLine(playerCharacters.Find(x => x.MapPosition == selectedTiles[tileNumber].MapPosition).Health);
            selectedTiles.Clear();
        }

        public override void Draw(List<Tile> selectedTiles, SpriteBatch spriteBatch) {
            selectedTiles.Where(x => x.Occupied == false).ToList().ForEach(x => x.Draw(spriteBatch, cross));
            selectedTiles.Where(x => x.Occupied == true).ToList().ForEach(x => x.Draw(spriteBatch, selectedTile));
        }
    }

    public class AttackMelee : Attack {
        public AttackMelee(Texture2D selectedTile, Texture2D cross) : base(selectedTile, cross) {

        }

        public override void PrepareSkill(List<PlayerCharacter> playerCharacters, int indexNumber, List<Tile> map, List<Tile> selectedTiles) {
            selectedTiles.AddRange(map.Where(x => Math.Abs(x.MapPosition.X - playerCharacters[indexNumber].MapPosition.X) + Math.Abs(x.MapPosition.Y - playerCharacters[indexNumber].MapPosition.Y) < playerCharacters[indexNumber].Range));

            selectedTiles.RemoveAll(x => x.MapPosition == playerCharacters[indexNumber].MapPosition);
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
        }

        public override void InvokeSkill(List<PlayerCharacter> playerCharacters, int indexNumber, List<Tile> map, List<Tile> selectedTiles, int tileNumber) {
            Console.WriteLine(playerCharacters.Find(x => x.MapPosition == selectedTiles[tileNumber].MapPosition).Health);
            playerCharacters.Where(x => x.MapPosition == selectedTiles[tileNumber].MapPosition).ToList().ForEach(x => x.Health = x.Health + 4);

            Console.WriteLine(playerCharacters.Find(x => x.MapPosition == selectedTiles[tileNumber].MapPosition).Health);
            selectedTiles.Clear();
        }
    }

    #endregion

    #region RangerSkills

    public class SkillRangerBomb : Skill {
        public SkillRangerBomb(Texture2D selectedTile) : base(selectedTile) {

        }

        public override void PrepareSkill(List<PlayerCharacter> playerCharacters, int indexNumber, List<Tile> map, List<Tile> selectedTiles) {
            selectedTiles.AddRange(map.Where(x => (x.MapPosition.Y == playerCharacters[indexNumber].MapPosition.Y ||
                                x.MapPosition.X == playerCharacters[indexNumber].MapPosition.X) &&
                                (Math.Abs(x.MapPosition.X - playerCharacters[indexNumber].MapPosition.X) + 
                                Math.Abs(x.MapPosition.Y - playerCharacters[indexNumber].MapPosition.Y) == playerCharacters[indexNumber].Range)));
        }

        public override void InvokeSkill(List<PlayerCharacter> playerCharacters, int indexNumber, List<Tile> map, List<Tile> selectedTiles, int tileNumber) {
            List<Tile> tempList = new List<Tile>();
            tempList.Add(map.Find(x => x.MapPosition.X == selectedTiles[tileNumber].MapPosition.X && x.MapPosition.Y == selectedTiles[tileNumber].MapPosition.Y));
            tempList.Add(map.Find(x => x.MapPosition.X == selectedTiles[tileNumber].MapPosition.X+1 && x.MapPosition.Y == selectedTiles[tileNumber].MapPosition.Y));
            tempList.Add(map.Find(x => x.MapPosition.X == selectedTiles[tileNumber].MapPosition.X-1 && x.MapPosition.Y == selectedTiles[tileNumber].MapPosition.Y));
            tempList.Add(map.Find(x => x.MapPosition.X == selectedTiles[tileNumber].MapPosition.X && x.MapPosition.Y+1 == selectedTiles[tileNumber].MapPosition.Y));
            tempList.Add(map.Find(x => x.MapPosition.X == selectedTiles[tileNumber].MapPosition.X && x.MapPosition.Y-1 == selectedTiles[tileNumber].MapPosition.Y));

            foreach (var player in playerCharacters) {
                if (tempList.Exists(x => x.MapPosition == player.MapPosition)) {
                    Console.WriteLine(player.Health);
                    player.Health -= 4;
                    Console.WriteLine(player.Health);
                }
            }

            selectedTiles.Clear();
        }
    }

    #endregion

    #region KnightSkills
   
    public class KnightSkillWhirlwind : Skill{

        public KnightSkillWhirlwind(Texture2D selectedTile) : base(selectedTile) {}

        public override void PrepareSkill(List<PlayerCharacter> playerCharacters, int indexNumber, List<Tile> map, List<Tile> selectedTiles) {
            selectedTiles.AddRange(map.Where(x => Math.Abs(x.MapPosition.X - playerCharacters[indexNumber].MapPosition.X) + Math.Abs(x.MapPosition.Y - playerCharacters[indexNumber].MapPosition.Y) < playerCharacters[indexNumber].SkillRange));

            selectedTiles.RemoveAll(x => x.MapPosition == playerCharacters[indexNumber].MapPosition);
        }

        public override void InvokeSkill(List<PlayerCharacter> playerCharacters, int indexNumber, List<Tile> map, List<Tile> selectedTiles, int tileNumber) {
            Console.WriteLine(playerCharacters.Find(x => x.MapPosition == selectedTiles[tileNumber].MapPosition).Health);
            playerCharacters.Where(x => x.MapPosition == selectedTiles[tileNumber].MapPosition).ToList().ForEach(x => x.Health = x.Health - playerCharacters[indexNumber].Damage);

            Console.WriteLine(playerCharacters.Find(x => x.MapPosition == selectedTiles[tileNumber].MapPosition).Health);
            selectedTiles.Clear();
        }

        public override void Draw(List<Tile> selectedTiles, SpriteBatch spriteBatch) {
            base.Draw(selectedTiles, spriteBatch);
        }
    }

    #endregion
}
