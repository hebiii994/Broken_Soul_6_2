using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class BossDialogue : MonoBehaviour
{
    [SerializeField] private TextMeshPro _dialogueText;
    [SerializeField] private float _tauntDuration = 4f;
    [SerializeField] private float _analysisDuration = 20f;

    [SerializeField] private string _defeatMonologue = "Patetico. Le faremo sapere, se sarai degno.";
    [SerializeField] private float _defeatMonologueDuration = 5f;

    [SerializeField] private GameObject _dialogueBubble;
    private string _lastTaunt = "";

    private void Start()
    {
        if (_dialogueBubble != null)
        {
            _dialogueBubble.SetActive(false);
        }
    }

    public Coroutine ShowIntroAnalysis()
    {
        return StartCoroutine(AnalysisRoutine());
    }

    public Coroutine ShowTaunt()
    {
        return StartCoroutine(TauntRoutine());
    }

    private IEnumerator AnalysisRoutine()
    {
        string analysis = GetIntroAnalysis();
        _dialogueText.text = analysis;
        _dialogueBubble.SetActive(true);
        yield return new WaitForSeconds(_analysisDuration);
        _dialogueBubble.SetActive(false);
    }

    private IEnumerator TauntRoutine()
    {
        string taunt = GetTauntBasedOnPlayerChoices();
        _dialogueText.text = taunt;
        _dialogueBubble.SetActive(true);
        yield return new WaitForSeconds(_tauntDuration);
        _dialogueBubble.SetActive(false);
    }

    public Coroutine ShowDefeatMonologue()
    {
        return StartCoroutine(DefeatRoutine());
    }

    private IEnumerator DefeatRoutine()
    {
        _dialogueText.text = _defeatMonologue;
        _dialogueBubble.SetActive(true);
        yield return new WaitForSeconds(_defeatMonologueDuration);
        _dialogueBubble.SetActive(false);
    }

    private string GetIntroAnalysis()
    {
        StringBuilder analysis = new StringBuilder("Dunque, vediamo chi abbiamo qui...\n\n");

        string merenda = NarrativeManager.Instance.GetInkVariable("scelta_merenda")?.ToString() ?? "";
        string parco = NarrativeManager.Instance.GetInkVariable("scelta_parco")?.ToString() ?? "";
        string scuola = NarrativeManager.Instance.GetInkVariable("scelta_scuola")?.ToString() ?? "";
        string sabato = NarrativeManager.Instance.GetInkVariable("scelta_sabato")?.ToString() ?? "";
        string universita = NarrativeManager.Instance.GetInkVariable("scelta_universita")?.ToString() ?? "";
        string contratto = NarrativeManager.Instance.GetInkVariable("scelta_contratto")?.ToString() ?? "";

        // Analisi dell'infanzia
        if (merenda == "panino")
            analysis.Append("Hai iniziato presto a sentirti 'diverso', eh? Un panino invece della merendina. Che piccolo ribelle.\n");
        else
            analysis.Append("Fin da piccolo, hai cercato l'approvazione del gruppo. Prevedibile.\n");

        // Analisi dell'adolescenza
        if (scuola == "incerta" && sabato == "prove")
            analysis.Append("La scuola 'artistica', la sala prove... ti sei nascosto dietro una presunta 'creativit�' per paura di affrontare il mondo reale.\n");
        else if (scuola == "sicura" && sabato == "casa")
            analysis.Append("Un percorso 'sicuro', ma passavi i sabati a casa. La paura del fallimento ti paralizza da sempre, vedo.\n");
        else
            analysis.Append("Hai fatto le scelte che ci si aspettava da te, senza mai chiederti cosa volessi davvero.\n");

        // Analisi dell'Et� Adulta
        if (universita == "sogno")
            analysis.Append("Hai persino inseguito un 'sogno' all'universit�. E guarda dove ti ha portato. Di fronte a me.\n");
        else if (universita == "presente")
            analysis.Append("L'indipendenza economica immediata. Una scorciatoia per evitare di pianificare, di impegnarti.\n");

        // Conclusione sul contratto
        if (contratto == "rifiuto")
            analysis.Append("E alla fine, hai persino avuto paura di accettare un'offerta. La paura di scegliere � la tua unica costante.\n");
        else
            analysis.Append("Hai accettato la prima cosa che ti � capitata, senza valore, senza prospettive.\n");

        analysis.Append("\nFrancamente, non sono impressionato. Dimostrami che mi sbaglio.");
        return analysis.ToString();
    }

    private string GetTauntBasedOnPlayerChoices()
    {
        var possibleTaunts = new List<string>();

        string merenda = NarrativeManager.Instance.GetInkVariable("scelta_merenda")?.ToString() ?? "";
        string parco = NarrativeManager.Instance.GetInkVariable("scelta_parco")?.ToString() ?? "";
        string scuola = NarrativeManager.Instance.GetInkVariable("scelta_scuola")?.ToString() ?? "";
        string sabato = NarrativeManager.Instance.GetInkVariable("scelta_sabato")?.ToString() ?? "";
        string universita = NarrativeManager.Instance.GetInkVariable("scelta_universita")?.ToString() ?? "";
        string contratto = NarrativeManager.Instance.GetInkVariable("scelta_contratto")?.ToString() ?? "";

        // Aggiungi frecciatine alla lista in base a ogni scelta
        if (merenda == "panino")
            possibleTaunts.Add("Ancora a fare l'anticonformista? Non ti ha mai portato da nessuna parte.");

        if (parco == "te_stesso")
            possibleTaunts.Add("Ti nascondi ancora come facevi al parco? Patetico.");

        if (scuola == "incerta")
            possibleTaunts.Add("La tua 'passione' ti ha portato qui. Ne � valsa la pena?");

        if (sabato == "casa")
            possibleTaunts.Add("Anche stasera preferiresti essere a casa, vero? Lontano da ogni giudizio.");

        if (universita == "sogno")
            possibleTaunts.Add("Anni a inseguire un sogno... per svegliarti nel mio ufficio.");

        if (contratto == "rifiuto")
            possibleTaunts.Add("Hai avuto paura di scegliere allora, e hai paura di combattere adesso. Non sei cambiato affatto.");

        // Se non ci sono frecciatine specifiche, aggiungi quelle di default
        if (possibleTaunts.Count == 0)
        {
            possibleTaunts.Add("Pensi davvero di essere all'altezza?");
            possibleTaunts.Add("Le tue esperienze non valgono nulla. Sei solo un altro candidato insignificante.");
            possibleTaunts.Add("Sei solo una delusione. Per te e per chi ha creduto in te.");
        }

        // Se c'� pi� di una frecciatina possibile e quella nuova � uguale a quella vecchia, prova a sceglierne un'altra
        if (possibleTaunts.Count > 1)
        {
            string newTaunt = possibleTaunts[Random.Range(0, possibleTaunts.Count)];
            if (newTaunt == _lastTaunt)
            {
                newTaunt = possibleTaunts[Random.Range(0, possibleTaunts.Count)]; // Seconda chance di sceglierne una diversa
            }
            _lastTaunt = newTaunt;
            return newTaunt;
        }

        // Se c'� solo una frecciatina, usa quella
        _lastTaunt = possibleTaunts[0];
        return possibleTaunts[0];
    }
}