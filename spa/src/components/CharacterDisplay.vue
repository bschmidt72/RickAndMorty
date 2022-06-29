<template>
  <div v-if="character" class="character-page">
    <h2 id="character-name">{{ props.character?.name }}</h2>
    <h5 id="character-location">Location: {{ props.character?.location?.name }}</h5>
    <h5 id="character-status">Status: {{ props.character?.status }}</h5>
    <img
      class="character-image"
      v-bind:alt="props.character?.name"
      v-bind:src="props.character?.image"
    />
  </div>
</template>

<script setup lang="ts">
import type { Character } from "@/model/Character";

const props = defineProps<{ character: Character | null }>();
</script>

<style scoped>
.character-page {
  display: flex;
  flex-direction: column;
  justify-content: flex-start;
  align-items: center;
  padding-bottom: 2em;
}
.character-image {
  padding-top: 1em;
  height: 300px;
  width: 300px;
  max-width: 100%;
  box-shadow: rgba(0, 0, 0, 0.25) 0 14px 28px, rgba(0, 0, 0, 0.22) 0 10px 10px;
}
@media screen and (max-width: 300px) {
  .character-image {
    height: auto;
    width: calc(100% - 60px);
  }
}
</style>
