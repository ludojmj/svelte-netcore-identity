<script>
  // StuffDelete.svelte
  import { onMount } from "svelte";
  import { useNavigate } from "svelte-navigator";
  import { apiGetStuffById, apiDeleteStuff } from "../api/stuff";
  import { accessToken, idToken } from "../oidc/components.module"; // "@dopry/svelte-oidc";
  import CommonForm from "./CommonForm.svelte";
  export let id;

  let stuffDatum = {};
  onMount(async () => {
    stuffDatum = await apiGetStuffById(id, $accessToken, $idToken);
  });

  const handleCancel = () => {
    navigate("/");
  };

  const navigate = useNavigate();
  const handleSubmit = async (event) => {
    const formData = new FormData(event.target);
    const updatedDatum = {};
    for (let field of formData) {
      const [key, value] = field;
      updatedDatum[key] = value;
    }

    stuffDatum = await apiDeleteStuff(id, $accessToken, $idToken);
    if (!stuffDatum.error) {
      navigate("/");
    }
  };
</script>

<main>
  <CommonForm
    title="Deleting a stuff"
    {stuffDatum}
    inputError={null}
    readonly={true}
    handleChange={null}
    {handleCancel}
    {handleSubmit}
  />
</main>
