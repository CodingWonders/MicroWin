name: Pull Request
description: File a structured PR to help us review faster
title: "[PR]: "
labels: ["needs-review"]
body:
  - type: markdown
    attributes:
      value: |
        Thanks for taking the time to contribute! 
  - type: checkboxes
    id: type-of-change
    attributes:
      label: Type of Change
      options:
        - label: New feature
        - label: Bug fix
        - label: Documentation update
        - label: UI/UX improvement
  - type: textarea
    id: description
    attributes:
      label: Description
      description: What does this PR do?
      placeholder: Summarize your changes here...
    validations:
      required: true
  - type: input
    id: issue-number
    attributes:
      label: Issue Related to PR
      description: List the issue number (e.g., #123)
      placeholder: "Resolves #"
    validations:
      required: true