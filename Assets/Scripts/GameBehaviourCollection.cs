using System;
using System.Collections.Generic;

// обновляет все сущности
[Serializable]
public class GameBehaviourCollection
{
    private List<GameBehaviour> _behaviours = new List<GameBehaviour>();

    public void Add(GameBehaviour behaviour) //для добавления врага
    {
        _behaviours.Add(behaviour);
    }

    public void GameUpdate() //для обновления всей коллекции
    {
        for (int i = 0; i < _behaviours.Count; i++)
        {
            if (!_behaviours[i].GameUpdate())
            {
                int lastIndex = _behaviours.Count - 1;
                _behaviours[i] = _behaviours[lastIndex];
                _behaviours.RemoveAt(lastIndex);
                i -= 1;
            }
        }
    }
}