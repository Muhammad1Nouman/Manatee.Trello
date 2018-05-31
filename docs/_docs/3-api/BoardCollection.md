---
title: BoardCollection
category: API
order: 11
---

A collection of boards.

**Assembly:** Manatee.Trello.dll

**Namespace:** Manatee.Trello

**Inheritance hierarchy:**

- Object
- ReadOnlyCollection&lt;IBoard&gt;
- ReadOnlyBoardCollection
- BoardCollection

## Methods

### Task&lt;[IBoard](../IBoard#iboard)&gt; Add(string name, IBoard source = null, CancellationToken ct = default(CancellationToken))

Creates a new board.

**Parameter:** name

The name of the board to create.

**Parameter:** source

A board to use as a template.

**Parameter:** ct

(Optional) A cancellation token for async processing.

**Returns:** The Manatee.Trello.IBoard generated by Trello.
