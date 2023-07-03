<script>
  import "./variables.scss";
  import { Router, Route } from "svelte-routing";
  import { crud } from "./lib/const.js";
  import { tokens } from "./lib/store.js";
  import AuthWrapper from "./components/oidc/AuthWrapper.svelte";
  import CrudManager from "./components/CrudManager.svelte";
  import StuffCreate from "./components/StuffCreate.svelte";
  import StuffRead from "./components/StuffRead.svelte";
  import StuffUpdate from "./components/StuffUpdate.svelte";
  import StuffDelete from "./components/StuffDelete.svelte";
  import Error from "./components/common/Error.svelte";
</script>

<AuthWrapper>
  <Router>
    <Route path={`/${crud.CREATE}`}>
      <StuffCreate />
    </Route>
    <Route path={`/${crud.READ}/:id`} let:params>
      <StuffRead id={params.id} />
    </Route>
    <Route path={`/${crud.UPDATE}/:id`} let:params>
      <StuffUpdate id={params.id} />
    </Route>
    <Route path={`/${crud.DELETE}/:id`} let:params>
      <StuffDelete id={params.id} />
    </Route>
    <Route path="/">
      <CrudManager />
    </Route>
    <Route>
      <Error msgErr="I got lost." hasReset={true} />
    </Route>
  </Router>
  <pre>{JSON.stringify($tokens, null, "\t")}</pre>
</AuthWrapper>
