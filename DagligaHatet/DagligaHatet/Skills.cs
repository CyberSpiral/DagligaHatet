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

        public abstract void PrepareSkill(Character turnMaster, List<Tile> map, List<Tile> selectedTiles);

        public abstract void InvokeSkill(Character turnMaster, List<Tile> map, List<Tile> selectedTiles, Tile clickedTile);

    }

    #region Attacks 

    public class Attack : Skill {
        protected Texture2D cross;


        /// <param name="selectedTile">Sword texture</param>
        /// <param name="cross">Cross texture</param>
        public Attack(Texture2D selectedTile, Texture2D cross) : base(selectedTile) {
            this.cross = cross;
        }

        public override void PrepareSkill(Character turnMaster, List<Tile> map, List<Tile> selectedTiles) {
        }

        public override void InvokeSkill(Character turnMaster, List<Tile> map, List<Tile> selectedTiles, Tile clickedTile) {
            World.DoDamage(turnMaster.Damage, turnMaster, clickedTile.Inhabitant);

            selectedTiles.Clear();
        }

        public virtual Tuple<List<Character>, bool> WouldHit(Character turnMaster, List<Tile> map, List<Tile> selectedTiles) {
            if (selectedTiles.Exists(x => x.Inhabited && x.Inhabitant.Alignment != turnMaster.Alignment)) {
                List<Character> potentialTarget = new List<Character>();
                selectedTiles.ForEach(x => {
                    if (x.Inhabited && x.Inhabitant.Alignment != turnMaster.Alignment) {
                        potentialTarget.Add(x.Inhabitant);
                    }
                });
                potentialTarget = potentialTarget.OrderBy(x => x.Health).ToList();
                return new Tuple<List<Character>, bool>(potentialTarget, true);
            }
            return new Tuple<List<Character>, bool>(new List<Character>(), false);
        }
    }

    public class AttackMelee : Attack {
        public AttackMelee(Texture2D selectedTile, Texture2D cross) : base(selectedTile, cross) {

        }

        public override void PrepareSkill(Character turnMaster, List<Tile> map, List<Tile> selectedTiles) {
            selectedTiles.AddRange(map.Where(x => (World.Distance(x.MapCoordinate, turnMaster.MapCoordinate)) < turnMaster.Range));
            selectedTiles.RemoveAll(x => x.MapCoordinate == turnMaster.MapCoordinate);
            base.PrepareSkill(turnMaster, map, selectedTiles);
        }

        public override Tuple<List<Character>, bool> WouldHit(Character turnMaster, List<Tile> map, List<Tile> selectedTiles) {
            PrepareSkill(turnMaster, map, selectedTiles);
            Tuple<List<Character>, bool> returnedData = base.WouldHit(turnMaster, map, selectedTiles);
            selectedTiles.Clear();

            return returnedData;
        }
    }

    public class AttackRangeCross : Attack {
        public AttackRangeCross(Texture2D selectedTile, Texture2D cross) : base(selectedTile, cross) {

        }

        public override void PrepareSkill(Character turnMaster, List<Tile> map, List<Tile> selectedTiles) {
            selectedTiles.AddRange(map.Where(x => Math.Abs(x.MapCoordinate.X - turnMaster.MapCoordinate.X) < turnMaster.Range && x.MapCoordinate.Y == turnMaster.MapCoordinate.Y ||
                                Math.Abs(x.MapCoordinate.Y - turnMaster.MapCoordinate.Y) < turnMaster.Range && x.MapCoordinate.X == turnMaster.MapCoordinate.X));

            selectedTiles.RemoveAll(x => x.MapCoordinate == turnMaster.MapCoordinate);
            base.PrepareSkill(turnMaster, map, selectedTiles);
        }
        public override Tuple<List<Character>, bool> WouldHit(Character turnMaster, List<Tile> map, List<Tile> selectedTiles) {
            PrepareSkill(turnMaster, map, selectedTiles);
            Tuple<List<Character>, bool> returnedData = base.WouldHit(turnMaster, map, selectedTiles);
            selectedTiles.Clear();

            return returnedData;
        }

    }

    public class AttackRangeXCross : Attack {
        Texture2D arrow;

        public AttackRangeXCross(Texture2D selectedTile, Texture2D arrow, Texture2D cross) : base(selectedTile, cross) {
            this.arrow = arrow;
        }

        public override void PrepareSkill(Character turnMaster, List<Tile> map, List<Tile> selectedTiles) {
            selectedTiles.AddRange(map.Where(x => Math.Abs(x.MapCoordinate.X - turnMaster.MapCoordinate.X) < turnMaster.Range &&
                                x.MapCoordinate.Y == turnMaster.MapCoordinate.Y ||
                                Math.Abs(x.MapCoordinate.Y - turnMaster.MapCoordinate.Y) < turnMaster.Range &&
                                x.MapCoordinate.X == turnMaster.MapCoordinate.X));
            selectedTiles.AddRange(map.Where(x => Math.Abs(x.MapCoordinate.X - turnMaster.MapCoordinate.X) == Math.Abs(x.MapCoordinate.Y - turnMaster.MapCoordinate.Y) &&
            World.Distance(x.MapCoordinate, turnMaster.MapCoordinate) < turnMaster.Range * 1.2));

            selectedTiles.RemoveAll(x => x.MapCoordinate == turnMaster.MapCoordinate);
            base.PrepareSkill(turnMaster, map, selectedTiles);
        }
        public override Tuple<List<Character>, bool> WouldHit(Character turnMaster, List<Tile> map, List<Tile> selectedTiles) {
            PrepareSkill(turnMaster, map, selectedTiles);
            Tuple<List<Character>, bool> returnedData = base.WouldHit(turnMaster, map, selectedTiles);
            selectedTiles.Clear();
            return returnedData;
        }

        public override void InvokeSkill(Character turnMaster, List<Tile> map, List<Tile> selectedTiles, Tile clickedTile) {
            DrawEngine.AddQueued("Arrow", arrow, turnMaster.MapCoordinate, Vector2.Zero, 0.5f, false, clickedTile.MapCoordinate);
            DrawEngine.AddDamageReport("", Color.White, Vector2.Zero, 0.5f, false, Vector2.Zero);
            base.InvokeSkill(turnMaster, map, selectedTiles, clickedTile);
        }

    }
    #endregion

    #region WizardSkills

    public class SkillWizardHeal : Skill {

        public SkillWizardHeal(Texture2D selectedTile) : base(selectedTile) {

        }

        public override void PrepareSkill(Character turnMaster, List<Tile> map, List<Tile> selectedTiles) {

            selectedTiles.AddRange(map.Where(x => x.Inhabited == true &&
            World.Distance(x.MapCoordinate, turnMaster.MapCoordinate) < turnMaster.SkillRange));
        }

        public override void InvokeSkill(Character turnMaster, List<Tile> map, List<Tile> selectedTiles, Tile clickedTile) {
            World.DoHealing(turnMaster.SkillDamage, turnMaster, clickedTile.Inhabitant);

            selectedTiles.Clear();
        }
    }

    #endregion

    #region RangerSkills

    public class SkillRangerBomb : Skill {
        Texture2D Target { get; }
        Texture2D arrow;
        Texture2D ex;

        public SkillRangerBomb(Texture2D selectedTile, Texture2D ex, Texture2D arrow, Texture2D target) : base(selectedTile) {
            Target = target;
            this.arrow = arrow;
            this.ex = ex;
        }

        public override void PrepareSkill(Character turnMaster, List<Tile> map, List<Tile> selectedTiles) {
            selectedTiles.AddRange(map.Where(x => (x.MapCoordinate.Y == turnMaster.MapCoordinate.Y ||
                                x.MapCoordinate.X == turnMaster.MapCoordinate.X) &&
                                (Math.Abs(x.MapCoordinate.X - turnMaster.MapCoordinate.X) +
                                Math.Abs(x.MapCoordinate.Y - turnMaster.MapCoordinate.Y) == turnMaster.Range)));

            selectedTiles.ForEach(x => {
                map.Where(y => World.Distance(y.MapCoordinate, x.MapCoordinate) < 2).ToList().Where(y => y.MapCoordinate != x.MapCoordinate).ToList().ForEach(y => {

                });
            });
        }

        public override void InvokeSkill(Character turnMaster, List<Tile> map, List<Tile> selectedTiles, Tile clickedTile) {

            DrawEngine.AddQueued("Arrow", arrow, turnMaster.MapCoordinate, Vector2.Zero, 0.5f, false, clickedTile.MapCoordinate);
            DrawEngine.AddQueued("Ex", ex, new Vector2(clickedTile.MapCoordinate.X - 1, clickedTile.MapCoordinate.Y - 1), Vector2.Zero, 0.0625f, 16, 1f, false);
            DrawEngine.AddDamageReport("", Color.White, Vector2.Zero, 1.5f, false, Vector2.Zero);
            List<Tile> tempList = new List<Tile>();
            tempList.AddRange(map.Where(x => World.Distance(x.MapCoordinate, clickedTile.MapCoordinate) < 2));
            tempList.ForEach(x => { if (x.Inhabited) World.DoDamage(turnMaster.SkillDamage, turnMaster, x.Inhabitant); });


            selectedTiles.Clear();
        }
    }

    #endregion

    #region KnightSkills

    public class SkillKnightWhirlwind : Skill {
        Texture2D whirl;
        public SkillKnightWhirlwind(Texture2D selectedTile, Texture2D Whirlwind) : base(selectedTile) { whirl = Whirlwind; }

        public override void PrepareSkill(Character turnMaster, List<Tile> map, List<Tile> selectedTiles) {
            selectedTiles.AddRange(map.Where(x => World.Distance(x.MapCoordinate, turnMaster.MapCoordinate) < turnMaster.SkillRange));

            selectedTiles.RemoveAll(x => x.MapCoordinate == turnMaster.MapCoordinate);

        }

        public override void InvokeSkill(Character turnMaster, List<Tile> map, List<Tile> selectedTiles, Tile clickedTile) {
            DrawEngine.AddQueued("Whirl", whirl, turnMaster.MapCoordinate, new Vector2(80, 80), 0.02f, 8, 0.32f, false);
            selectedTiles.ForEach(x => {
                if (x.Inhabited) {
                    World.DoDamage(turnMaster.SkillDamage, turnMaster, x.Inhabitant);
                }
            });

            DrawEngine.AddPause(0.2f);
            DrawEngine.AddReverseQueued("Whirl", whirl, turnMaster.MapCoordinate, new Vector2(80, 80), 0.02f, 8, 0.32f, false);

            selectedTiles.ForEach(x => {
                if (x.Inhabited) {
                    World.DoDamage(turnMaster.SkillDamage, turnMaster, x.Inhabitant);
                }
            });

            selectedTiles.Clear();
        }
    }

    #endregion
}
