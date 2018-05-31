---
title: IAction
category: API
order: 52
---

Documents all of the activities in Trello.

**Assembly:** Manatee.Trello.dll

**Namespace:** Manatee.Trello

**Inheritance hierarchy:**

- IAction

## Properties

### DateTime CreationDate { get; }

Gets the creation date of the action.

### [IMember](../IMember#imember) Creator { get; }

Gets the member who performed the action.

### [IActionData](../IActionData#iactiondata) Data { get; }

Gets any data associated with the action.

### DateTime? Date { get; }

Gets the date and time at which the action was performed.

### [ActionType](../ActionType#actiontype)? Type { get; }

Gets the type of action.

## Events

### Action&lt;[IAction](../IAction#iaction), IEnumerable&lt;string&gt;&gt; Updated

Raised when data on the action is updated.

## Methods

### Task Delete(CancellationToken ct = default(CancellationToken))

Deletes the action.

**Parameter:** ct

#### Remarks

This permanently deletes the action from Trello&#39;s server, however, this object will remain in memory and all properties will remain accessible.

### Task Refresh(CancellationToken ct = default(CancellationToken))

Refreshes the action data.

**Parameter:** ct

(Optional) A cancellation token for async processing.
