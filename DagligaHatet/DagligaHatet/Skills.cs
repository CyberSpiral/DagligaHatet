using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DagligaHatet {

    public enum attackStyle {
        around, linecross, lineXcross
    }

    public abstract class Skill {

        public abstract void PrepareSkill(List<PlayerCharacter> playerCharacters, int indexNumber, List<Tile> map, List<Tile> selectedTiles);

        public abstract void InvokeSkill(List<PlayerCharacter> playerCharacters, int indexNumber, List<Tile> map, List<Tile> selectedTiles, int tileNumber);
    }

    #region Attacks 

    public class Attack : Skill {
        public override void PrepareSkill(List<PlayerCharacter> playerCharacters, int indexNumber, List<Tile> map, List<Tile> selectedTiles) {

            throw new NotImplementedException();
        }

        public override void InvokeSkill(List<PlayerCharacter> playerCharacters, int indexNumber, List<Tile> map, List<Tile> selectedTiles, int tileNumber) {
            Console.WriteLine(playerCharacters.Find(x => x.MapPosition == selectedTiles[tileNumber].MapPosition).Health);
            playerCharacters.Where(x => x.MapPosition == selectedTiles[tileNumber].MapPosition).ToList().ForEach(x => x.Health = x.Health - playerCharacters[indexNumber].Damage);

            Console.WriteLine(playerCharacters.Find(x => x.MapPosition == selectedTiles[tileNumber].MapPosition).Health);
            selectedTiles.Clear();
        }
    }

    public class AttackMelee : Attack {
        public override void PrepareSkill(List<PlayerCharacter> playerCharacters, int indexNumber, List<Tile> map, List<Tile> selectedTiles) {
            selectedTiles.AddRange(map.Where(x => Math.Abs(x.MapPosition.X - playerCharacters[indexNumber].MapPosition.X) + Math.Abs(x.MapPosition.Y - playerCharacters[indexNumber].MapPosition.Y) < playerCharacters[indexNumber].Range));

            selectedTiles.RemoveAll(x => x.MapPosition == playerCharacters[indexNumber].MapPosition);
        }
    }

    public class AttackRangeCross : Attack {
        public override void PrepareSkill(List<PlayerCharacter> playerCharacters, int indexNumber, List<Tile> map, List<Tile> selectedTiles) {
            selectedTiles.AddRange(map.Where(x => Math.Abs(x.MapPosition.X - playerCharacters[indexNumber].MapPosition.X) < playerCharacters[indexNumber].Range &&
                                x.MapPosition.Y == playerCharacters[indexNumber].MapPosition.Y ||
                                Math.Abs(x.MapPosition.Y - playerCharacters[indexNumber].MapPosition.Y) < playerCharacters[indexNumber].Range &&
                                x.MapPosition.X == playerCharacters[indexNumber].MapPosition.X));

            selectedTiles.RemoveAll(x => x.MapPosition == playerCharacters[indexNumber].MapPosition);
        }
    }

    public class AttackRangeXCross : Attack {
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

    #region MageSkills

    public class HealMageSkill : Skill {
        public override void PrepareSkill(List<PlayerCharacter> playerCharacters, int indexNumber, List<Tile> map, List<Tile> selectedTiles) {
            selectedTiles.AddRange(map.Where(x => x.Occupied == true &&
            (Math.Abs(x.MapPosition.X - playerCharacters[indexNumber].MapPosition.X)
            + Math.Abs(x.MapPosition.Y - playerCharacters[indexNumber].MapPosition.Y) < playerCharacters[indexNumber].SkillRange)));
        }

        public override void InvokeSkill(List<PlayerCharacter> playerCharacters, int indexNumber, List<Tile> map, List<Tile> selectedTiles, int tileNumber) {

        }
    }

    #endregion
}
