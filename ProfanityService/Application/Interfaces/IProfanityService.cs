using ProfanityService.Application.DTO;
using ProfanityService.Domain;

namespace ProfanityService.Application.Interfaces;

/*
Notation:
  Banned         — set of banned words from the profanity database
  ValidText(t)   — t ≠ null
  ValidWord(w)   — w ≠ null ∧ w.Trim() ≠ "" ∧ w.Length ≤ 100
  wholeMatch(t, w)
      — w occurs in t as a whole word (\b boundaries, ignore case)
Global invariants:
  I1  ∀ w ∈ Banned : ValidWord(w)
  I2  ∀ w1, w2 ∈ Banned : (w1.Word = w2.Word) ⇒ (w1 = w2)
*/
public interface IProfanityService
{
    /*
    CheckTextAsync(dto)
    Pre:
      dto ≠ null ∧
      ValidText(dto.Text)
    Post:
      result ≠ null ∧
      result.OriginalText = dto.Text ∧
      result.ContainsProfanity ⇔ (|result.DetectedWords| > 0) ∧
      (∀ w ∈ result.DetectedWords :
          ∃ b ∈ Banned : b.Word = w ∧ wholeMatch(dto.Text, b.Word)) ∧
      (∀ b ∈ Banned :
          wholeMatch(dto.Text, b.Word) ⇒
            (b.Word ∈ result.DetectedWords ∧
             that occurrence in result.FilteredText equals
             ('*' repeated |b.Word| times))) ∧
      (¬ ∃ b ∈ Banned : wholeMatch(dto.Text, b.Word)) ⇒
        (result.FilteredText = dto.Text ∧
         result.DetectedWords = ∅ ∧
         result.ContainsProfanity = false)
    */
    Task<CheckTextResultDto> CheckTextAsync(CheckTextDto dto);
    Task<List<ProfanityWord>> GetAllWordsAsync();
    Task<ProfanityWord> AddWordAsync(AddWordDto dto);
    Task<bool> DeleteWordAsync(Guid id);
}
