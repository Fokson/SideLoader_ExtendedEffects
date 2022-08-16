using SideLoader;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OutwardModTemplate
{

  namespace OutwardModTemplate
  {
    public class SL_AddAllyStatusEffect : SL_AddStatusEffect, ICustomModel
    {
      public Type SLTemplateModel => typeof( SL_AddAllyStatusEffect );
      public Type GameModel => typeof( SLEx_AddAllyStatusEffect );

      public float Range = 9999f;

      public override void ApplyToComponent<T>( T component )
      {
        SLEx_AddAllyStatusEffect sLEx_AddAllyStatusEffect = component as SLEx_AddAllyStatusEffect;

        StatusEffect status = ResourcesPrefabManager.Instance.GetStatusEffectPrefab( this.StatusEffect );

        if ( !status ) {
          SL.LogWarning( $"{this.GetType().Name}: Could not find any effect with the identifier '{this.StatusEffect}'" );
          return;
        }

        sLEx_AddAllyStatusEffect.Status = status;
        sLEx_AddAllyStatusEffect.NoDealer = NoDealer;
        sLEx_AddAllyStatusEffect.ChanceToContract = ChanceToContract;
        sLEx_AddAllyStatusEffect.AffectController = AffectController;
        sLEx_AddAllyStatusEffect.AdditionalLevel = AdditionalLevel;
        sLEx_AddAllyStatusEffect.Range = Range;
      }

      public override void SerializeEffect<T>( T effect )
      {
        SLEx_AddAllyStatusEffect sLEx_AddAllyStatusEffect = effect as SLEx_AddAllyStatusEffect;

        if ( sLEx_AddAllyStatusEffect.Status ) {
          StatusEffect = sLEx_AddAllyStatusEffect.Status.IdentifierName;
          ChanceToContract = sLEx_AddAllyStatusEffect.ChanceToContract;
          AffectController = sLEx_AddAllyStatusEffect.AffectController;
          AdditionalLevel = sLEx_AddAllyStatusEffect.AdditionalLevel;
          NoDealer = sLEx_AddAllyStatusEffect.NoDealer;
        }
      }
    }

    public class SLEx_AddAllyStatusEffect : Effect, ICustomModel
    {
      public Type SLTemplateModel => typeof( SL_AddAllyStatusEffect );
      public Type GameModel => typeof( SLEx_AddAllyStatusEffect );

      public StatusEffect Status;
      public int ChanceToContract = 100;
      public bool AffectController = false;
      public int AdditionalLevel = 0;
      public float Range = 9999f;
      public bool NoDealer = false;

      public override void ActivateLocally( Character _affectedCharacter, object[] _infos )
      {
        List<Character> CharactersAroundAffected = new List<Character>();
        Character ChosenCharacter = null;
        float distanceToChosen = 99999999f;
        CharacterManager.Instance.FindCharactersInRange( _affectedCharacter.transform.position, Range, ref CharactersAroundAffected );

        foreach ( var foundcharacter in CharactersAroundAffected ) {
          if ( foundcharacter.Faction != Character.Factions.Player ) {
            continue;
          }
          float distanceToFound = 99999999f;
          if ( foundcharacter == _affectedCharacter ) {
            if ( !AffectController )
              continue;
            distanceToFound = 9998f;
          } else 
            distanceToFound = (float)Math.Sqrt( Vector3.Distance( _affectedCharacter.transform.position, foundcharacter.transform.position ) );
          if ( distanceToFound < distanceToChosen ) {
            ChosenCharacter = foundcharacter;
            distanceToChosen = distanceToFound;
          }
        }
        if ( ChosenCharacter ) {
          bool flag = ChanceToContract >= 100;
          if ( !flag )
            flag = Random.Range( 0, 100 ) + 1 > 100 - ChanceToContract;
          if ( flag )
            ChosenCharacter.StatusEffectMngr.AddStatusEffect( Status, !NoDealer ? SourceCharacter : (Character)null );
        }
      }
    }
  }
}
