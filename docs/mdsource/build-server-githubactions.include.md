Use a [if: failure()](https://docs.github.com/en/free-pro-team@latest/actions/reference/context-and-expression-syntax-for-github-actions#failure) condition to upload any `*.received.*` files if the build fails.

```yaml
- name: Upload Test Results
  if: failure()
  uses: actions/upload-artifact@v2
  with:
    name: verify-test-results
    path: |
      **/*.received.*
```