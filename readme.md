# Project Genesis: “Pogadajmy”

**Pogadajmy** was born from the need to build a modern, lightweight, and scalable communication platform—a Polish take on Discord or Slack, but simpler and friendlier to communities. The core goal is real‑time conversations—both in groups (rooms) and privately (DM rooms)—with full data control and easy integration with systems people already use.

## Technology foundation

The backend is built on .NET 9 using SignalR (WebSocket communication), PostgreSQL, and Redis (cache + presence tracking). The architecture is modular: a REST API handles sign‑up, authorization, room management, and message history, while the SignalR layer powers real‑time communication. JWT is used for auth, and Redis serves as the backplane to scale connections across multiple server instances.

The current frontend is experimental (HTML + JS, SignalR client), but the target is a production web app (Nuxt or React) and a mobile app (React Native / Flutter).

## Idea and context

Unlike corporate tools such as Teams, Slack, or Discord, Pogadajmy aims to be lightweight, private, and built for communities and small businesses. It’s not just a chat—it’s a **communication ecosystem** designed to connect:

* real‑time conversations (SignalR),
* webhook integrations (Trily‑like automation),
* user presence and activity (presence, typing),
* simple moderation (flagging, soft‑delete, telemetry, analytics).

## Business model

1. **Freemium + SaaS for teams**

   * Free plan for individuals or groups up to 10 members.
   * Paid (Pro) plan with extras: unlimited history, webhook integrations (GitHub, Notion, Jira), activity stats, dedicated hosting.

2. **White‑label / on‑premise**

   * Run your own Pogadajmy instance for companies, schools, or communities.
   * Licensed or subscription model with per‑user / per‑instance pricing.

3. **Integration marketplace**

   * Planned community integrations (bots, notifications, AI assistant) with a revenue share on premium integrations.

4. **Data and telemetry (ethical analytics)**

   * Built‑in telemetry for reports (e.g., top posters, peak activity) without commercial tracking of users.

## Key differentiators

* Native SignalR and WebSocket support—near‑zero latency.
* Event‑driven architecture with a Redis backplane—ready to scale.
* Simple setup, local hosting (Docker‑first approach).
* Open‑source mindset with a path to SaaS.
* Extensible with AI components (e.g., content moderation, smart replies).

## Long‑term goal

Create a **Polish alternative to Slack and Discord** with a transparent monetization model and the option to self‑host. The aim is an open‑core product—part open‑source (community edition), part commercial (enterprise / Pro edition). The open version invites community contributions, while paid features (e.g., AI transcription, monitoring, premium integrations) fund ongoing development.

## How it makes money

* Pro / Business subscriptions (PLN 10–25 per user per month).
* Managed hosting and enterprise support (B2B SaaS, white‑label).
* Selling team analytics and insights (ethical telemetry).

## In short

**Pogadajmy** is a scalable chat system with the ambition to become the backbone of team communication in Poland and beyond. It blends a love for real‑time tech (SignalR, Redis, WebSocket) with a philosophy of simplicity and openness—showing that a modern communicator can be lightweight, secure, and local.
