<script>
  // Error.svelte
  import { useNavigate } from "svelte-navigator";
  export let msgErr;

  const navigate = useNavigate();
  const handleReset = async () => {
    navigate("/");
  };
</script>

<main>
  <div class="alert alert-danger row" role="alert">
    <div class="col-9">{msgErr}</div>
    <a class="btn btn-outline-info col-3" href="/" role="button">Reset</a>
  </div>
</main>
