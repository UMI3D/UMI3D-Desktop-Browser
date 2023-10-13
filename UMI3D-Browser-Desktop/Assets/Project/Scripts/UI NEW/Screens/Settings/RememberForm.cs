using System.Linq;
using umi3d.baseBrowser.preferences;
using umi3d.common.interaction;
using UnityEngine;

public class RememberForm
{
    private const string m_Directory = "Form";

    public void SaveAnswer(FormAnswerDto pAnswer)
    {
        if (pAnswer == null)
        {
            Debug.LogError("Form Answer should not be null to be saved");
            return;
        }

        PreferencesManager.StoreData(pAnswer, pAnswer.id.ToString(), m_Directory);
    }

    public FormAnswerDto GetAnswer(FormAnswerDto pTemplate)
    {
        if (pTemplate.answers == null) return null;

        PreferencesManager.TryGet(out FormAnswerDto answer, pTemplate.id.ToString(), m_Directory);
        
        if (answer == null) return null;
        if (answer.answers == null) return null;
        if (!CheckAnswerValidity(pTemplate, answer)) return null;

        return answer;
    }

    private bool CheckAnswerValidity(FormAnswerDto pTemplate, FormAnswerDto pAnswer) 
        => pTemplate.answers.Select(a => a.id).All(pAnswer.answers.Select(a => a.id).Contains) && pTemplate.answers.Count == pAnswer.answers.Count;
}