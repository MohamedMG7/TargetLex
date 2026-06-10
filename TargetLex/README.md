# TargetLex

TargetLex is a .NET console tool for generating targeted password wordlists.

Use it only against accounts, systems, and assessments where you have explicit permission.

## Example

```bash
dotnet run --project TargetLex.csproj -- \
  --target-name Alice \
  --target-nickname ali \
  --birth-year 1998 \
  --birth-month 04 \
  --birth-day 21 \
  --leet basic \
  --min-length 6 \
  --max-length 18 \
  --max-results 5000 \
  --preview \
  --output-file alice-demo
```

Common options:

- `--leet off|basic|advanced` controls leetspeak variants.
- `--output-dir` chooses where the wordlist is written.
- `--min-length` and `--max-length` filter generated candidates.
- `--max-results` caps output size.
- `--preview` prints the first generated candidates.
- `--no-animation` disables the banner and progress bar for automation.
