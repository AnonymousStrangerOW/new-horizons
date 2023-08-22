import { defineConfig } from "astro/config";
import starlight from "@astrojs/starlight";

import sitemap from "@astrojs/sitemap";
import { generateSchema } from "./src/schema_generator";

generateSchema("body_schema.json");

// https://astro.build/config
export default defineConfig({
    compressHTML: true,
    integrations: [
        starlight({
            title: "New Horizons",
            description:
                "Documentation on how to use the New Horizons planet creation tool for Outer Wilds.",
            defaultLocale: "en-us",
            customCss: ["/src/styles/custom.css"],
            logo: {
                src: "/src/assets/icon.webp",
                alt: "The New Horizons Logo"
            },
            social: {
                github: "https://github.com/Outer-Wilds-New-Horizons/new-horizons",
                discord: "https://discord.gg/wusTQYbYTc"
            },
            sidebar: [
                {
                    label: "Start Here",
                    autogenerate: {
                        directory: "start-here"
                    }
                },
                {
                    label: "Guides",
                    autogenerate: {
                        directory: "guides"
                    }
                },
                {
                    label: "Schemas",
                    items: [
                        { label: "Celestial Body Schema", link: "schemas/body-schema" },
                        { label: "Star System Schema", link: "schemas/star-system-schema" },
                        { label: "Translation Schema", link: "schemas/translation-schema" },
                        { label: "Addon Manifest Schema", link: "schemas/addon-manifest-schema" },
                        { label: "Dialogue Schema", link: "schemas/dialogue-schema" },
                        { label: "Text Schema", link: "schemas/text-schema" },
                        { label: "Ship Log Schema", link: "schemas/shiplog-schema" }
                    ]
                },
                {
                    label: "Reference",
                    autogenerate: {
                        directory: "reference"
                    }
                }
            ]
        })
    ],
    // Process images with sharp: https://docs.astro.build/en/guides/assets/#using-sharp
    image: {
        service: {
            entrypoint: "astro/assets/services/sharp"
        }
    }
});
