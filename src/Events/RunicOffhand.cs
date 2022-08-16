using SideLoader;
using System;
using UnityEngine.Serialization;

namespace SideLoader_ExtendedEffects.Events
{
  public class SL_RunicOffhand : SL_RunicBlade, ICustomModel
  {
    public Type SLTemplateModel => typeof( SL_RunicOffhand );
    public Type GameModel => typeof( SLEx_RunicOffhand );

    public override void ApplyToComponent<T>( T component )
    {
      SLEx_RunicOffhand sLEx_RunicOffhand = component as SLEx_RunicOffhand;

      sLEx_RunicOffhand.SummonLifeSpan = SummonLifespan;
      if ( ResourcesPrefabManager.Instance.GetItemPrefab( WeaponID ) is Weapon weapon ) {
        sLEx_RunicOffhand.RunicBladePrefab = weapon;
      } else {
        SL.Log( "SL_RunicBlade: Could not get an Item with ID '" + WeaponID + "'!" );
        return;
      }

      if ( ResourcesPrefabManager.Instance.GetItemPrefab( GreaterWeaponID ) is Weapon greaterWeapon ) {
        sLEx_RunicOffhand.RunicGreatBladePrefab = greaterWeapon;
      } else {
        SL.Log( "SL_RunicBlade: Could not get an Item with ID '" + GreaterWeaponID + "'!" );
        return;
      }

      if ( ResourcesPrefabManager.Instance.GetEffectPreset( PrefixImbueID ) is ImbueEffectPreset imbue ) {
        sLEx_RunicOffhand.ImbueAmplifierRunicBlade = imbue;
      } else {
        SL.Log( "SL_RunicBlade: Could not get an imbue with the ID '" + PrefixImbueID + "'!" );
        return;
      }

      if ( ResourcesPrefabManager.Instance.GetEffectPreset( PrefixGreaterImbueID ) is ImbueEffectPreset greatImbue ) {
        sLEx_RunicOffhand.ImbueAmplifierGreatRunicBlade = greatImbue;
      } else {
        SL.Log( "SL_RunicBlade: Could not get an imbue with the ID '" + PrefixGreaterImbueID + "'!" );
        return;
      }
    }

    public override void SerializeEffect<T>( T effect )
    {
      SLEx_RunicOffhand sLEx_RunicOffhand = effect as SLEx_RunicOffhand;

      SummonLifespan = sLEx_RunicOffhand.SummonLifeSpan;
      WeaponID = sLEx_RunicOffhand.RunicBladePrefab.ItemID;
      GreaterWeaponID = sLEx_RunicOffhand.RunicGreatBladePrefab.ItemID;
      PrefixImbueID = sLEx_RunicOffhand.ImbueAmplifierRunicBlade.PresetID;
      PrefixGreaterImbueID = sLEx_RunicOffhand.ImbueAmplifierGreatRunicBlade.PresetID;
    }
  }

  public class SLEx_RunicOffhand : Effect, ICustomModel
  {
    public Weapon RunicBladePrefab;
    public Weapon RunicGreatBladePrefab;
    public ImbueEffectPreset ImbueAmplifierRunicBlade;
    public ImbueEffectPreset ImbueAmplifierGreatRunicBlade;
    [FormerlySerializedAs( "LifeSpan" )]
    public float SummonLifeSpan = 180f;

    public Type SLTemplateModel => typeof( SL_RunicOffhand );
    public Type GameModel => typeof( SLEx_RunicOffhand );

    public override void ActivateLocally( Character _affectedCharacter, object[] _infos )
    {
      Console.WriteLine( "ActivateLocally: RunicOffand" );
      Console.WriteLine( "* ID: " + RunicBladePrefab.m_localizedName );

      if ( !_affectedCharacter )
        return;
      if ( RunicBladePrefab && !_affectedCharacter.Inventory.HasEquipped( RunicBladePrefab.ItemID ) ) {
        Weapon weapon = ItemManager.Instance.GenerateItem( RunicBladePrefab.ItemID ) as Weapon;
        weapon.SetHolderUID( _affectedCharacter.UID + "_" + RunicBladePrefab.name );
        weapon.ClientGenerated = PhotonNetwork.isNonMasterClientInRoom;
        weapon.SetKeepAlive();
        Item equippedItem1 = _affectedCharacter.Inventory.Equipment.GetEquippedItem( EquipmentSlot.EquipmentSlotIDs.RightHand );
        Item equippedItem2 = _affectedCharacter.Inventory.Equipment.GetEquippedItem( EquipmentSlot.EquipmentSlotIDs.LeftHand );
        weapon.GetComponent<SummonedEquipment>().Activate( EnvironmentConditions.ConvertToGameTime( SummonLifeSpan ), equippedItem1 ? equippedItem1.UID : null, equippedItem2 ? equippedItem2.UID : null );
        if ( ImbueAmplifierRunicBlade && _affectedCharacter.Inventory.SkillKnowledge.IsItemLearned( 8205200 ) )
          weapon.AddImbueEffect( ImbueAmplifierRunicBlade, SummonLifeSpan );
        if ( equippedItem1 ) {
          _affectedCharacter.Inventory.UnequipItem( (Equipment)equippedItem1 );
          equippedItem1.ForceUpdateParentChange();
        }
        if ( equippedItem2 ) {
          _affectedCharacter.Inventory.UnequipItem( (Equipment)equippedItem2 );
          equippedItem2.ForceUpdateParentChange();
        }
        weapon.transform.SetParent( _affectedCharacter.Inventory.GetMatchingEquipmentSlot( EquipmentSlot.EquipmentSlotIDs.LeftHand ).transform );
        weapon.ForceStartInit();
      }
    }
  }
}
