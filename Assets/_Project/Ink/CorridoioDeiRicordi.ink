VAR scelta_merenda = ""
VAR scelta_parco = ""
VAR scelta_scuola = ""
VAR scelta_sabato = ""
VAR scelta_universita = ""
VAR scelta_contratto = ""

-> Inizio

=== Inizio ===
"Non avere fretta. Rispondi onestamente. Prova a ricordare cosa era davvero importante per te, in quel momento... Ricomincia da qui. Dai piccoli bivi che hanno tracciato il sentiero. La prima, piccola pressione. Il bisogno di essere come gli altri... cosa scegliesti quel giorno?" #voce
-> Bivio_1_Merenda

=== Bivio_1_Merenda ===
* [La Merendina]
    ~ scelta_merenda = "merendina"
    "...già da allora. Bastava così poco per sentirsi parte di qualcosa. Per non essere quello strano." #pensiero
    -> Bivio_2_Parco
* [Il Panino]
    ~ scelta_merenda = "panino"
     "...già da allora. Diverso. Anche solo per un panino. Non sapevo ancora se sarebbe stata una costante." #pensiero
    -> Bivio_2_Parco

=== Bivio_2_Parco ===
"Il pallone rotolava via. Potevi rincorrerlo, unirti al gioco... o potevi restare a guardare le formiche, perso nel tuo mondo. Dove sei andato?" #voce
* [Verso gli Altri]
    ~ scelta_parco = "altri"
    "Correre con loro. Anche se non capivo le regole. L'importante era non restare indietro." #pensiero
    -> Bivio_3_Scuola
* [Verso Te Stesso]
    ~ scelta_parco = "te_stesso"
    "Le formiche avevano più senso. Avevano uno scopo. A loro non dovevo chiedere il permesso per giocare." #pensiero
    -> Bivio_3_Scuola

=== Bivio_3_Scuola ===
"La prima, grande scelta imposta. Il primo mattone del tuo futuro... o della tua gabbia. Quale percorso ti hanno convinto a prendere?" #voce
* [La Via Sicura]
    ~ scelta_scuola = "sicura"
    "Dicevano che mi avrebbe aperto tutte le porte. Non mi hanno detto che dietro ce ne sarebbero state altre, tutte uguali."#pensiero
    -> Bivio_4_Sabato
* [La Via Pratica]
    ~ scelta_scuola = "pratica"
    "Un lavoro sicuro. Era quello che contava. È ancora quello che conta, giusto?" #pensiero
    -> Bivio_4_Sabato
* [La Via Incerta]
    ~ scelta_scuola = "incerta"
    "Ho seguito un sogno. E da allora non ho mai smesso di sentirmi in colpa." #pensiero
    -> Bivio_4_Sabato

=== Bivio_4_Sabato ===
"Arrivava il rito del sabato. La stessa piazza, le stesse facce. Cercavi il tuo posto in mezzo al rumore, o ti bastava il tuo?" #voce
* [In Piazza]
    ~ scelta_sabato = "piazza"
    "L'ansia di dire la cosa giusta, di avere le scarpe giuste. Era diventata un ossessione, DEVO essere abbastanza per gli altri.." #pensiero
    -> Bivio_5_Universita
* [A Casa]
    ~ scelta_sabato = "casa"
    "Dicevo che preferivo un film, o una partita online. La verità è che avevo paura di non essere invitato." #pensiero
    -> Bivio_5_Universita
* [In Sala Prove]
    ~ scelta_sabato = "prove"
    "Il rumore che facevamo noi era diverso. Era nostro. Peccato che a vederlo fossimo soltanto noi stessi." #pensiero
    -> Bivio_5_Universita

=== Bivio_5_Universita ===
"E dopo il diploma? La grande promessa. Hai seguito il percorso tracciato da quella prima scelta, o hai provato a deviare?" #voce
* [Il Futuro Garantito]
    ~ scelta_universita = "garantito"
    "Anni a studiare cose che non amavo, per un futuro che non è mai arrivato." #pensiero
    -> Bivio_6_Contratto
* [Il Sogno]
    ~ scelta_universita = "sogno"
    "Anni a studiare quello che amavo, per sentirmi dire che non valeva niente." #pensiero
    -> Bivio_6_Contratto
* [Il Presente]
    ~ scelta_universita = "presente"
    "Guadagnare subito. Indipendenza. Così la chiamavo. Ora non so più cosa sia." #pensiero
    -> Bivio_6_Contratto

=== Bivio_6_Contratto ===
"E alla fine, eccoti qui. Un numero tra tanti. L'ennesima firma su un foglio che vale meno di ieri e più di domani. Quale sapore aveva la tua rassegnazione?" #voce
* [Contratto a Termine]
    ~ scelta_contratto = "termine"
   "Una data di scadenza. Come uno yogurt. Almeno sai quando andrai a male." #pensiero
    -> Fine_Intro
* [Partita IVA]
    ~ scelta_contratto = "p_iva"
    "'Libero professionista'. La più grande bugia che mi sia mai raccontato." #pensiero
    -> Fine_Intro
* [Un Altro Colloquio]
    ~ scelta_contratto = "rifiuto"
   "Ho ringraziato e ho detto che ci avrei pensato. Non l'ho mai fatto. Forse la paura di scegliere era più forte della paura di non avere niente." #pensiero
    -> Fine_Intro

=== Fine_Intro ===
#evento:StartGame
-> END
=== Ignored_Once ===
"Non vuoi ricordare? O credi che non scegliere sia una scelta?" #voce
-> END

=== Ignored_Twice ===
"Interessante. Rifiuti il sentiero. Ma anche questo è un sentiero. Il più battuto di tutti." #voce
-> END

=== Ignored_Thrice ===
"Non c'è nulla, qui. Solo il buio che ti sei scelto..." #voce
-> END

=== Ignored_Fourth ===
"Continui. Pensi ci sia qualcosa alla fine di questo corridoio?" #voce
-> END

=== Ignored_Fifth ===
"C'è solo quello che hai lasciato indietro." #voce
-> END

=== Finale_Segreto ===
// Questo nodo non ha testo, solo un tag per attivare la fine del gioco in Unity
#evento:SecretEnding
-> END
