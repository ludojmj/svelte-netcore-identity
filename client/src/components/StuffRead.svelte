<script>
  // StuffRead.svelte
  import { onMount } from "svelte";
  import { useNavigate } from "svelte-navigator";
  import { apiGetStuffById } from "../api/stuff";
  import { accessToken, idToken } from "../oidc/components.module"; // "@dopry/svelte-oidc";
  import CommonForm from "./CommonForm.svelte";
  export let id;

  let stuffDatum = {};
  onMount(async () => {
    stuffDatum = await apiGetStuffById(id, $accessToken, $idToken);
  });

  const navigate = useNavigate();
  const handleCancel = () => {
    navigate("/");
  };
</script>

<main>
  <CommonForm
    title="Reading a stuff"
    {stuffDatum}
    inputError={null}
    disabled="disabled"
    handleChange={null}
    {handleCancel}
    handleSubmit={handleCancel}
  />
</main>
