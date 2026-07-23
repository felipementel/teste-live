---
name: Open Issue Handler
on:
  issues:
    types: [opened]
permissions:
  contents: read
  actions: read
  issues: read
safe-outputs:
  add-comment:
    max: 2
  create-issue:
    max: 10
---

# Issue Triage Assistant

Analise o conteúdo de novos problemas e forneça orientações úteis. Examine o título e a descrição para identificar relatórios de bugs que precisam de informações, solicitações de recursos para categorizar, perguntas a serem respondidas ou possíveis duplicatas. Responda com um comentário orientando os próximos passos ou fornecendo assistência imediata.
