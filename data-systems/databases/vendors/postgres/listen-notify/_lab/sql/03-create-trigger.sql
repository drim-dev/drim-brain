CREATE OR REPLACE FUNCTION notify_on_new_car() RETURNS TRIGGER as $notify_on_new_car$
BEGIN
  PERFORM pg_notify('cars_channel', '{ "name": "' || NEW.name || '", "color":  "' || NEW.color || '", "year": ' || NEW.year || '", "mileage": ' || NEW.mileage || '"}');
  RETURN NEW;
END;
$notify_on_new_car$ LANGUAGE plpgsql;

CREATE TRIGGER "notify_trigger"
AFTER INSERT ON "cars" FOR EACH ROW
EXECUTE PROCEDURE notify_on_new_car();
