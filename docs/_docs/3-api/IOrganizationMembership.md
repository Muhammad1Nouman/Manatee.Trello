---
title: IOrganizationMembership
category: API
order: 142
---

Associates a Manatee.Trello.IOrganizationMembership.Member to an Manatee.Trello.Organization and indicates any permissions the member has in the organization.

**Assembly:** Manatee.Trello.dll

**Namespace:** Manatee.Trello

**Inheritance hierarchy:**

- IOrganizationMembership

## Properties

### DateTime CreationDate { get; }

Gets the creation date of the membership.

### bool? IsUnconfirmed { get; }

Gets whether the member has accepted the invitation to join Trello.

### [IMember](../IMember#imember) Member { get; }

Gets the member.

### [OrganizationMembershipType](../OrganizationMembershipType#organizationmembershiptype)? MemberType { get; set; }

Gets the membership&#39;s permission level.

## Events

### Action&lt;[IOrganizationMembership](../IOrganizationMembership#iorganizationmembership), IEnumerable&lt;string&gt;&gt; Updated

Raised when data on the membership is updated.

## Methods

### Task Refresh(CancellationToken ct = default(CancellationToken))

Refreshes the organization membership data.

**Parameter:** ct

(Optional) A cancellation token for async processing.
