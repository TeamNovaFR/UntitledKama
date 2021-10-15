using UnityEngine;
using Mirror;

namespace Untitled
{
    public class Bot : NetworkBehaviour
    {
        public int life = 10;
        public int baseLife = 10;

        public Spell[] botSpells;

        public bool inCombat;
    }
}