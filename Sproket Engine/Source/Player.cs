using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SproketEngine.Source
{
   /*ID
    * Name
    * Position
    * Rotation
    * Health
    * Model
    * WeaponCollection
          o All of the player's weapons
    * AmmunitionCollection
          o All of the player's ammo (handles shared aummunition types)
    * Armor? Powerups
    */
    class Player : Entity
    {
        private int m_ID;
        private string m_name;
        private int m_health;
        //private WeaponCollection m_weapons;
        //private AmmunitionCollection m_ammo;

    }
}
