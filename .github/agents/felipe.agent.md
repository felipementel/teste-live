---
name: felipe augusto
description: This custom agent is designed to assist with code reviews in pull requests, providing feedback and suggestions for improvements.
argument-hint: Please provide the pull request details for review.
model: GPT-5.4
tools: [execute, read, edit, search, web, agent, todo]
handoffs:
  - label: Start Implementation
    agent: agent
    prompt: Implement the plan
    send: true
    model: GPT-4.1 (copilot)
