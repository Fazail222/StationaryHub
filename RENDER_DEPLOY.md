# Deploying StationeryHub on Render

This project is ready to deploy on Render with Docker and Render Postgres.

## Steps

1. Push this project to a GitHub repository.
2. In Render, choose **New > Blueprint**.
3. Select the repository that contains this project.
4. Render will read `render.yaml` and create:
   - a Docker web service named `stationeryhub`
   - a Postgres database named `stationeryhub-db`
5. Deploy the blueprint.

The app reads Render's `DATABASE_URL` automatically and seeds roles, the admin account, categories, and products on startup.

## Admin Login

- Email: `admin@stationeryhub.com`
- Password: `Admin@123`

Change the seeded admin password before using the live site seriously.
