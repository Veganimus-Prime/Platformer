﻿// Aaron Grincewicz Veganimus@icloud.com 6/5/2021
using System.Collections;
using UnityEngine;
namespace Veganimus.Platformer
{
    public class EnemyWeapon : Weapon
    {
        public bool IsShooting { get; set; }
        protected override IEnumerator Start()
        {
            yield return new WaitForSeconds(1.0f);
            _shootCoolDown = new WaitForSeconds(_fireRate);
            _secondaryCoolDown = new WaitForSeconds(_secondaryFireRate);
            _poolManager = PoolManager.Instance;
            _pmTransform = _poolManager.transform;
        }

        protected override void Update()
        {
            if (Time.time > _canFire)
                Shoot();
        }

        protected override void Shoot()
        {
            if (IsShooting)
            {
                _canFire = Time.time + _fireRate;
                Instantiate(_bulletPrefab, _fireOffset.position, _fireOffset.rotation, _pmTransform);
                
                StartCoroutine(ShootCoolDownRoutine());
            }
        }
        protected override void SecondaryShoot(){}
    }
}