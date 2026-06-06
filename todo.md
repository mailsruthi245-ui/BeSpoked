# BeSpoked Bikes - TODO

## Gaps to Address

- [x] 1. **Discount Management** — Added `DiscountsController` (GET, POST, PUT, DELETE) and `DiscountList.tsx` UI with create/edit/delete. Frontend fully migrated to TypeScript (TSX) with shared types in `src/types.ts`.

- [x] 2. **Create Customer UI** — Added "+ New Customer" button and modal form to `CustomerList.tsx`. Added `createCustomer` to `api.ts`.

- [x] 3. **Create Salesperson UI** — Added "+ New Salesperson" button and modal to `SalespersonList.tsx`. Added `createSalesperson` to `api.ts`. Same modal reused for create and edit (id=0 = new).

- [x] 4. **Backend Validation: Terminated Salesperson on Sale** — Added salesperson lookup and termination check in `SalesController.Create()`. Returns 400 if salesperson is terminated.

- [x] 5. **Quarterly Bonus — Made Configurable** — Bonus rules now driven by `appsettings.json` (`QuarterlyBonus:TopN`, `QuarterlyBonus:BonusPercentage`). Bound via `QuarterlyBonusSettings` and injected into `SalesController`. Default: top 1 earner gets 10% bonus.

- [x] 6. **Publish to Online Repository** — Published at https://github.com/mailsruthi245-ui/BeSpoked/tree/main
