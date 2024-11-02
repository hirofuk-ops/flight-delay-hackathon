<script lang="ts">
    import type { Airport } from "$lib";
    export let data;

    export let form;

    const airports: Airport[] = data.airports;

    // create an array of days for display
    // include the name starting with Monday and a value of 1
    // end with Sunday as 7
    const days = Array.from({length: 7}, (_, i) => {
        return {
            name: new Intl.DateTimeFormat('ja-JP', { weekday: 'long'}).format(new Date(0, 0, i + 1)),
            value: i + 1
        }
    });

    if(form && form.result) {
        console.log(form.result);
    }
</script>

<h1>Welcome to GitHub Copilot Flight Delay Hackathon</h1>

<form method="POST" action="?/getDelay">

    <!-- create dropdown list of airports with id as key -->
    <select name="airport">
        {#each airports as airport (airport.id)}
            <option value={airport.id}>{airport.name}</option>
        {/each}
    </select>

    <!-- create dropdown list of days with value as key -->
    <select name="day">
        {#each days as day (day.value)}
            <option value={day.value}>{day.name}</option>
        {/each}
    </select>
    <br>

    <button type="submit">遅延の検索</button>
</form>

<br />

{#if form && form.result}
    <div>{Math.round(form.result.delay * 100)}% の確率で遅延が発生します。 確実性: {Math.round(form.result.certainty * 100)}% </div>
{/if}