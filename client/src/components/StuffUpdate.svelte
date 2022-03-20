<script>
  // StuffUpdate.svelte
  import { onMount } from "svelte";
  import { useNavigate } from "svelte-navigator";
  import { apiGetStuffById, apiUpdateStuff } from "../api/stuff";
  import { accessToken, idToken } from "../oidc/components.module"; // "@dopry/svelte-oidc";
  import CommonForm from "./CommonForm.svelte";
  export let id;

  let inputError = "";
  let initialDatum = {};
  let stuffDatum = {};
  onMount(async () => {
    initialDatum = await apiGetStuffById(id, $accessToken, $idToken);
    stuffDatum = initialDatum;
  });

  const handleCancel = () => {
    navigate("/");
  };

  const handleChange = (event) => {
    const { name, value } = event.target;
    stuffDatum = { ...stuffDatum, [name]: value };
    inputError = "";
  };

  const navigate = useNavigate();
  const handleSubmit = async (event) => {
    const formData = new FormData(event.target);
    let hasChanged = false;
    for (let field of formData) {
      const [key, value] = field;
      stuffDatum[key] = value;
      if (!hasChanged && value !== initialDatum[key]) {
        hasChanged = true;
      }
    }

    if (!hasChanged) {
      inputError = "No significant changes...";
      setTimeout(() => {
        inputError = "";
      }, 2000);

      return;
    }

    if (!/\S/.test(stuffDatum.label)) {
      inputError = "The label cannot be empty.";
      setTimeout(() => {
        inputError = "";
      }, 2000);

      return;
    }

    stuffDatum = await apiUpdateStuff(id, stuffDatum, $accessToken, $idToken);
    if (stuffDatum.error) {
      inputError = stuffDatum.error;
    } else {
      navigate("/");
    }
  };
</script>

<main>
  <CommonForm
    title="Updating a stuff"
    stuffDatum={initialDatum}
    {inputError}
    readonly={false}
    {handleChange}
    {handleCancel}
    {handleSubmit}
  />
</main>
