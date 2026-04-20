using System;

namespace CityZero.Data.RuntimeDTOs
{
    [Serializable]
    public sealed class WeaponData
    {
        public string id;
        public string displayName;
        public string category;
        public int damage;
        public float fireRate;
        public int magazineSize;
        public float reloadTime;
        public float spreadBase;
        public float spreadMove;
        public float recoil;
        public float range;
        public float suppression;
        public float armorPenetration;
        public float threatNoise;
        public string ammoType;
        public WeaponEconomyData economy;
        public string[] tags;
    }

    [Serializable]
    public sealed class WeaponEconomyData
    {
        public int buyPrice;
        public int sellPrice;
        public int ammoBoxPrice;
    }
}
