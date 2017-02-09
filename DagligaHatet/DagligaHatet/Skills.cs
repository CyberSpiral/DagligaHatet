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

        public abstract void PrepareSkill(PlayerCharacter turnMaster, List<Tile> map, List<Tile> selectedTiles);

        public abstract void InvokeSkill(PlayerCharacter turnMaster, List<Tile> map, List<Tile> selectedTiles, Tile clickedTile);
        
    }

    #region Attacks 

    public class Attack : Skill {
        protected Texture2D cross;


        /// <param name="selectedTile">Sword texture</param>
        /// <param name="cross">Cross texture</param>
        public Attack(Texture2D selectedTile, Texture2D cross) : base(selectedTile) {
            this.cross = cross;
        }

        public override void PrepareSkill(PlayerCharacter turnMaster, List<Tile> map, List<Tile> selectedTiles) {
            selectedTiles.Where(x => !x.Inhabited).ToList().ForEach(x => DrawEngine.AddPermanent("selectedTiles", cross, x.MapPosition));
            selectedTiles.Where(x => x.Inhabited).ToList().ForEach(x => DrawEngine.AddPermanent("selectedTiles", selectedTile, x.MapPosition));
        }

        public override void InvokeSkill(PlayerCharacter turnMaster, List<Tile> map, List<Tile> selectedTiles, Tile clickedTile) {
            Console.WriteLine(clickedTile.Inhabitant.Health);
            clickedTile.Inhabitant.Health -= turnMaster.Damage;
            Console.WriteLine(clickedTile.Inhabitant.Health);
            DrawEngine.ClearPermanent("selectedTiles");
            selectedTiles.Clear();
        }
        
        public Tuple<PlayerCharacter,bool> WouldHit(PlayerCharacter turnMaster, List<Tile> map, List<Tile> selectedTiles) {
            if (selectedTiles.Exists(x => x.Inhabited && x.Inhabitant.Alignment != turnMaster.Alignment)) {
                List<PlayerCharacter> potentialTarget = new List<PlayerCharacter>();
                selectedTiles.ForEach(x => {
                    if (x.Inhabited) {
                        potentialTarget.Add(x.Inhabitant);
                    }
                });
                potentialTarget = potentialTarget.OrderBy(x => x.Health).ToList();
                return new Tuple<PlayerCharacter, bool>(potentialTarget.Last(), true);
            }
            return new Tuple<PlayerCharacter, bool>(null, false);
        }
    }

    public class AttackMelee : Attack {
        public AttackMelee(Texture2D selectedTile, Texture2D cross) : base(selectedTile, cross) {

        }

        public override void PrepareSkill(PlayerCharacter turnMaster, List<Tile> map, List<Tile> selectedTiles) {
            selectedTiles.AddRange(map.Where(x => World.Distance(x.MapPosition, turnMaster.MapPosition) < turnMaster.Range));
            selectedTiles.RemoveAll(x => x.MapPosition == turnMaster.MapPosition);
            base.PrepareSkill(turnMaster, map, selectedTiles);
        }
    }

    public class AttackRangeCross : Attack {
        public AttackRangeCross(Texture2D selectedTile, Texture2D cross) : base(selectedTile, cross) {

        }

        public override void PrepareSkill(PlayerCharacter turnMaster, List<Tile> map, List<Tile> selectedTiles) {
            selectedTiles.AddRange(map.Where(x => Math.Abs(x.MapPosition.X - turnMaster.MapPosition.X) < turnMaster.Range && x.MapPosition.Y == turnMaster.MapPosition.Y ||
                                Math.Abs(x.MapPosition.Y - turnMaster.MapPosition.Y) < turnMaster.Range && x.MapPosition.X == turnMaster.MapPosition.X));

            selectedTiles.RemoveAll(x => x.MapPosition == turnMaster.MapPosition);
            base.PrepareSkill(turnMaster, map, selectedTiles);
        }
    }

    public class AttackRangeXCross : Attack {
        public AttackRangeXCross(Texture2D selectedTile, Texture2D cross) : base(selectedTile, cross) {

        }

        public override void PrepareSkill(PlayerCharacter turnMaster, List<Tile> map, List<Tile> selectedTiles) {
            selectedTiles.AddRange(map.Where(x => Math.Abs(x.MapPosition.X - turnMaster.MapPosition.X) < turnMaster.Range &&
                                x.MapPosition.Y == turnMaster.MapPosition.Y ||
                                Math.Abs(x.MapPosition.Y - turnMaster.MapPosition.Y) < turnMaster.Range &&
                                x.MapPosition.X == turnMaster.MapPosition.X));
            selectedTiles.AddRange(map.Where(x => Math.Abs(x.MapPosition.X - turnMaster.MapPosition.X) == Math.Abs(x.MapPosition.Y - turnMaster.MapPosition.Y) &&
            World.Distance(x.MapPosition,turnMaster.MapPosition) < turnMaster.Range * 1.2));

            selectedTiles.RemoveAll(x => x.MapPosition == turnMaster.MapPosition);
            base.PrepareSkill(turnMaster, map, selectedTiles);
        }
    }
    #endregion

    #region WizardSkills

    public class SkillWizardHeal : Skill {

        public SkillWizardHeal(Texture2D selectedTile) : base(selectedTile) {

        }

        public override void PrepareSkill(PlayerCharacter turnMaster, List<Tile> map, List<Tile> selectedTiles) {

            selectedTiles.AddRange(map.Where(x => x.Inhabited == true &&
            World.Distance(x.MapPosition,turnMaster.MapPosition) < turnMaster.SkillRange));
            selectedTiles.ForEach(x => DrawEngine.AddPermanent("selectedTiles", selectedTile, x.MapPosition, Vector2.Zero, 0.3f, 2));
        }

        public override void InvokeSkill(PlayerCharacter turnMaster, List<Tile> map, List<Tile> selectedTiles, Tile clickedTile) {
            Console.WriteLine(clickedTile.Inhabitant.Health);
            clickedTile.Inhabitant.Health += turnMaster.SkillDamage;

            Console.WriteLine(clickedTile.Inhabitant.Health);
            DrawEngine.ClearPermanent("selectedTiles");
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

        public override void PrepareSkill(PlayerCharacter turnMaster, List<Tile> map, List<Tile> selectedTiles) {
            selectedTiles.AddRange(map.Where(x => (x.MapPosition.Y == turnMaster.MapPosition.Y ||
                                x.MapPosition.X == turnMaster.MapPosition.X) &&
                                (Math.Abs(x.MapPosition.X - turnMaster.MapPosition.X) +
                                Math.Abs(x.MapPosition.Y - turnMaster.MapPosition.Y) == turnMaster.Range)));

            selectedTiles.ForEach(x => {
                map.Where(y => World.Distance(y.MapPosition, x.MapPosition) < 2).ToList().Where(y => y.MapPosition != x.MapPosition).ToList().ForEach(y => {
                    DrawEngine.AddPermanent("selectedTiles", selectedTile, y.MapPosition);
                });
                DrawEngine.AddPermanent("selectedTiles", Target, x.MapPosition);
            });
        }

        public override void InvokeSkill(PlayerCharacter turnMaster, List<Tile> map, List<Tile> selectedTiles, Tile clickedTile) {
            List<Tile> tempList = new List<Tile>();
            tempList.AddRange(map.Where(x => World.Distance(x.MapPosition, clickedTile.MapPosition) < 2));
            tempList.ForEach(x => { if (x.Inhabited) x.Inhabitant.Health -= turnMaster.SkillDamage; });

            DrawEngine.ClearPermanent("selectedTiles");
            selectedTiles.Clear();
        }
    }

    #endregion

    #region KnightSkills

    public class SkillKnightWhirlwind : Skill {
        Texture2D whirl;
        public SkillKnightWhirlwind(Texture2D selectedTile, Texture2D Whirlwind) : base(selectedTile) { whirl = Whirlwind; }

        public override void PrepareSkill(PlayerCharacter turnMaster, List<Tile> map, List<Tile> selectedTiles) {
            selectedTiles.AddRange(map.Where(x => World.Distance(x.MapPosition,turnMaster.MapPosition) < turnMaster.SkillRange));

            selectedTiles.RemoveAll(x => x.MapPosition == turnMaster.MapPosition);
            selectedTiles.ForEach(x => DrawEngine.AddPermanent("selectedTiles", selectedTile, x.MapPosition));

        }

        public override void InvokeSkill(PlayerCharacter turnMaster, List<Tile> map, List<Tile> selectedTiles, Tile clickedTile) {
            DrawEngine.AddQueued("Whirl", whirl, turnMaster.MapPosition, new Vector2(80, 80), 0.02f, 8, 0.32f, false);
            selectedTiles.ForEach(x => {
                if (x.Inhabited) {
                    Console.WriteLine(x.Inhabitant.Health);
                    x.Inhabitant.Health -= turnMaster.SkillDamage;
                    Console.WriteLine(x.Inhabitant.Health);
                }});

            DrawEngine.AddPause(0.2f);
            DrawEngine.AddReverseQueued("Whirl", whirl, turnMaster.MapPosition, new Vector2(80, 80), 0.02f, 8, 0.32f, false);

            selectedTiles.ForEach(x => {
                if (x.Inhabited) {
                    Console.WriteLine(x.Inhabitant.Health);
                    x.Inhabitant.Health -= turnMaster.SkillDamage;
                    Console.WriteLine(x.Inhabitant.Health);
                }
            });
            DrawEngine.ClearPermanent("selectedTiles");
            selectedTiles.Clear();
        }
    }

    #endregion
}
