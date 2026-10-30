 //Why OrderItem stores its own price:
 OrderItem stores its own Price column because the price of a product can change over time. 
 If OrderItem read the product's current price instead, past orders would show incorrect totals whenever a product's price is updated.
 
